using AudioToolbox;
using AVFoundation;
using BSE.Tunes.XApp.iOS.Services;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Foundation;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlayerService))]
namespace BSE.Tunes.XApp.iOS.Services
{
    public class PlayerService : IPlayerService
    {
        private Task _currentTask;
        private AudioQueueTimeline _audioQueueTimeline = null;
        private StreamingPlayback _player;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _playerOutputFinished;
        private long _totalStreamLength;

        private IRequestService _requestService => DependencyService.Resolve<IRequestService>();
        private ISettingsService _settingsService => DependencyService.Resolve<ISettingsService>();

        public AudioPlayerState AudioPlayerState { get; private set; } = AudioPlayerState.Closed;
        public float Progress => GetProgress();

        public event Action<AudioPlayerState> AudioPlayerStateChanged;
        public event Action<MediaState> MediaStateChanged;

        public void Pause()
        {
            _player?.Pause();
        }

        public void Play()
        {
            _player?.Play();
        }

        public void Stop()
        {
            _player?.Stop();
        }

        public void SetTrack(Track track)
        {
            if (track != null)
            {
                if (_currentTask != null)
                {
                    if (_player != null)
                    {
                        _player.OutputReady -= OnPlayerOutputReady;
                        _player.Finished -= OnPlayerFinished;
                        _player.AudioPlayerStateChanged -= OnAudioPlayerStateChanged;
                    }
                    if (_cancellationTokenSource != null)
                    {
                        _cancellationTokenSource.Cancel();
                        _cancellationTokenSource.Dispose();
                        _cancellationTokenSource = null;
                    }
                    _currentTask.Wait();
                    _currentTask.Dispose();
                    _currentTask = null;
                }

                _cancellationTokenSource = new CancellationTokenSource();
                Guid guid = track.Guid;
                //guid = new Guid("f856d83e-cf83-425f-b2e2-254ad704e766");
                // Create a shared intance session & check
                using (var session = AVAudioSession.SharedInstance())
                {
                    if (session != null)
                    {
                        _currentTask = Task.Run(() => StreamDownloadHandler(guid, _cancellationTokenSource.Token));

                        // Set up the session for playback category
                        session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.DefaultToSpeaker);
                        session.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out NSError error);
                    }
                }
            }
        }

        private async Task StreamDownloadHandler(Guid guid, CancellationToken cancellationToken)
        {
            var buffer = new byte[8192];
            long totalNumberBytesRead = -1;

            _playerOutputFinished = false;
            _totalStreamLength = 0L;
            _audioQueueTimeline = null;

            if (_player != null)
            {
                _player.OutputReady -= OnPlayerOutputReady;
                _player.Finished -= OnPlayerFinished;
                _player.AudioPlayerStateChanged -= OnAudioPlayerStateChanged;
                _player?.FlushAndClose();
                _player = null;
            }

            Console.WriteLine($"Downloader started");
            try
            {
                using (_player = new StreamingPlayback())
                {
                    _player.OutputReady += OnPlayerOutputReady;
                    _player.Finished += OnPlayerFinished;
                    _player.AudioPlayerStateChanged += OnAudioPlayerStateChanged;

                    Uri requestUri = GetRequestUri(guid);
                    using (var httpClient = await _requestService.GetHttpClient(true))
                    {
                        using (var response = await TryGetAsync(0, requestUri, httpClient, cancellationToken))
                        {
                            _totalStreamLength = response.Content.Headers.ContentLength.GetValueOrDefault(totalNumberBytesRead) - 1;
                            Console.WriteLine($"Stream Length: {_totalStreamLength}");

                            using (var inputStream = await response.Content.ReadAsStreamAsync())
                            {
                                do
                                {
                                    var bytesReceived = await inputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                                    //if (bytesReceived == 0)
                                    //{
                                    //    Console.WriteLine($"0 bytesReceived at {totalNumberBytesRead} of {totalStreamLength}");
                                    //}
                                    //Console.WriteLine($"{ totalNumberBytesRead  } + {bytesReceived } from { totalStreamLength}");
                                    _player.ParseBytes(buffer, bytesReceived, false, totalNumberBytesRead == (int)_totalStreamLength);

                                    totalNumberBytesRead += bytesReceived;
                                } while (!_playerOutputFinished || cancellationToken.IsCancellationRequested);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine($"Exception thrown in {nameof(StreamDownloadHandler)} with message {exception.Message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception thrown in {nameof(StreamDownloadHandler)} with message {exception.Message}");
            }
        }

        private static async Task<HttpResponseMessage> TryGetAsync(int attempt, Uri requestUri, HttpClient httpClient, CancellationToken cancellationToken)
        {
            var responseMessage = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (attempt == 5)
            {
                throw new HttpRequestException("GetAsync aborted after 5 attempts");
            }
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Attempt no {attempt} in {nameof(TryGetAsync)}");
                await Task.Delay(1000);
                return await TryGetAsync(attempt += 1, requestUri, httpClient, cancellationToken);
            }
            return responseMessage;
        }

        private void OnPlayerOutputReady(AudioToolbox.OutputAudioQueue outputAudioQueue)
        {
            Console.WriteLine($"Player: Output Ready");
            
            _audioQueueTimeline = outputAudioQueue.CreateTimeline();
            MediaStateChanged(MediaState.Opened);
        }

        private void OnPlayerFinished(object sender, EventArgs e)
        {
            Console.WriteLine($"Player: Output Finished");
            _playerOutputFinished = true;
            MediaStateChanged(MediaState.Ended);
        }

        private void OnAudioPlayerStateChanged(AudioPlayerState audioPlayerState)
        {
            if (audioPlayerState != AudioPlayerState)
            {
                AudioPlayerState = audioPlayerState;
                AudioPlayerStateChanged(audioPlayerState);
            }
        }

        private Uri GetRequestUri(Guid guid)
        {
            var builder = new UriBuilder(_settingsService.ServiceEndPoint);
            builder.Path = Path.Combine(builder.Path, $"/api/files/audio/{guid}");
            return builder.Uri;
        }
        
        private float GetProgress()
        {
            var queue = _player?.OutputQueue;
            if (queue == null || _audioQueueTimeline == null)
            {
                return default;
            }

            bool timelineDiscontinuty = false;
            var audioTimeStamp = new AudioTimeStamp();

            queue.GetCurrentTime(
                _audioQueueTimeline,
                ref audioTimeStamp,
                ref timelineDiscontinuty); ;

            return (float)(audioTimeStamp.SampleTime
                / _player?.OutputQueue.SampleRate
                / (_totalStreamLength * 8 / _player.BitRate));
        }
    }
}