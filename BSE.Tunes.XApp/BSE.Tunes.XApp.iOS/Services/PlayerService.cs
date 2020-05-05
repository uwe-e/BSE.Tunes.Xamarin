using AVFoundation;
using BSE.Tunes.XApp.iOS.Services;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Foundation;
using System;
using System.Collections.Concurrent;
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
        private StreamingPlayback _player;
        private CancellationTokenSource _cancellationTokenSource;
        bool _playerOutputFinished;
        private IRequestService _requestService => DependencyService.Resolve<IRequestService>();
        private ISettingsService _settingsService => DependencyService.Resolve<ISettingsService>();

        public AudioPlayerState AudioPlayerState { get; private set; } = AudioPlayerState.Closed;

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

        public void PlayTracks(ObservableCollection<int> trackIds, AudioPlayerMode audioplayerMode)
        {
            throw new NotImplementedException();
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
                //using (var session = AVAudioSession.SharedInstance())
                //{
                //if (session != null)
                {
                    _currentTask = Task.Run(() => StreamDownloadHandler(guid, _cancellationTokenSource.Token));

                    //_tasks.Add(t);
                    //await Task.Run(() => StreamDownloadHandler(guid));
                    // Set up the session for playback category
                    //NSError error;
                    //session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.DefaultToSpeaker);
                    //session.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out error);
                }
                //}
            }
            //return null;
        }
        
        private async Task StreamDownloadHandler(Guid guid, CancellationToken cancellationToken)
        {
            var buffer = new byte[8192];
            long totalNumberBytesRead = -1;

            _playerOutputFinished = false;

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
                        using (var response = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                        {
                            long totalStreamLength = response.Content.Headers.ContentLength.GetValueOrDefault(totalNumberBytesRead) - 1;
                            Console.WriteLine($"Stream Length: {totalStreamLength}");

                            using (var inputStream = await response.Content.ReadAsStreamAsync())
                            {
                                do
                                {
                                    var bytesReceived = await inputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                                    if (bytesReceived == 0)
                                    {
                                        Console.WriteLine($"0 bytesReceived at {totalNumberBytesRead} of {totalStreamLength}");
                                    }
                                    //Console.WriteLine($"{ totalNumberBytesRead  } + {bytesReceived } from { totalStreamLength}");
                                    _player.ParseBytes(buffer, bytesReceived, false, totalNumberBytesRead == (int)totalStreamLength);

                                    totalNumberBytesRead += bytesReceived;
                                } while (!_playerOutputFinished || cancellationToken.IsCancellationRequested);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception thrown in {nameof(StreamDownloadHandler)} with message {exception.Message}");
            }
        }

        private void OnPlayerOutputReady(AudioToolbox.OutputAudioQueue obj)
        {
            //sampleRate = _player.OutputQueue.SampleRate;
            //obj.
            //AudioPlayerStateChanged(AudioPlayerState.Opening);
            Console.WriteLine($"Player: Output Ready ");
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
    }
}