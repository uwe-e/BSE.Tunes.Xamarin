using AudioToolbox;
using AVFoundation;
using BSE.Tunes.XApp.iOS.Audio;
using BSE.Tunes.XApp.iOS.IO;
using BSE.Tunes.XApp.iOS.Services;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Net.Http;
using BSE.Tunes.XApp.Services;
using Foundation;
using MediaPlayer;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlayerService))]
namespace BSE.Tunes.XApp.iOS.Services
{
    public class PlayerService : IPlayerService
    {
        private Task _currentTask;
        private AudioQueueTimeline _audioQueueTimeline;
        private StreamingPlayback _player;
        private CancellationTokenSource _cancellationTokenSource;
        private long _totalStreamLength;

        private IRequestService _requestService => DependencyService.Resolve<IRequestService>();
        private ISettingsService _settingsService => DependencyService.Resolve<ISettingsService>();

        public AudioPlayerState AudioPlayerState { get; private set; } = AudioPlayerState.Closed;
        public float Progress => GetProgress();

        public event Action<AudioPlayerState> AudioPlayerStateChanged;
        public event Action<MediaState> MediaStateChanged;
        
        public Task<bool> CloseAsync()
        {
            return CleanUpPlayerResources();
        }

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
            SetTrack(track, null);
        }
        
        public async void SetTrack(Track track, Uri coverUri)
        {
            if (track != null)
            {
                if (await CleanUpPlayerResources())
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    Guid guid = track.Guid;
                    //guid = new Guid("f856d83e-cf83-425f-b2e2-254ad704e766");
                    // Create a shared intance session & check
                    using (var session = AVAudioSession.SharedInstance())
                    {
                        if (session != null)
                        {
                            //_currentTask = Task.Run(() => StreamDownloadHandler(guid, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
                            _currentTask = Task.Factory.StartNew(() => StreamDownloadHandler(guid, _cancellationTokenSource.Token), _cancellationTokenSource.Token);

                            // Set up the session for playback category
                            session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.DefaultToSpeaker);
                            session.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out NSError error);
                            //session.SetActive(true);

                            SetNowPlayingInfo(track, coverUri);
                        }
                    }
                }
            }
        }

        private void SetNowPlayingInfo(Track track, Uri coverUri)
        {
            // Moved to ExtendedTabbedRenderer because of a UIKit Consistency error: you are calling a UIKit method that can only be invoked from the UI thread.
            //UIKit.UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();

            MPNowPlayingInfo np = new MPNowPlayingInfo();
            np.Title = track.Name;
            np.Artist = track.Album.Artist.Name;
            np.AlbumTitle = track.Album.Title;
            if (coverUri != null)
            {
                using (var data = NSData.FromUrl(coverUri))
                {
                    if (data != null)
                    {
                        np.Artwork = new MPMediaItemArtwork(UIImage.LoadFromData(data));
                    }
                }
            }

            MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = np;
        }

        private Task<bool> CleanUpPlayerResources()
        {
            if (_currentTask != null)
            {
                if (_player != null)
                {
                    Console.WriteLine($"{nameof(CleanUpPlayerResources)} release player resources");
                    _player.OutputReady -= OnPlayerOutputReady;
                    _player.Finished -= OnPlayerFinished;
                    _player.AudioPlayerStateChanged -= OnAudioPlayerStateChanged;
                }
                if (_cancellationTokenSource != null)
                {
                    Console.WriteLine($"{nameof(CleanUpPlayerResources)} cancel token");
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
                _currentTask.Wait();
                _currentTask.Dispose();
                _currentTask = null;
            }
            return Task.FromResult(true);
        }

        private async Task StreamDownloadHandler(Guid guid, CancellationToken cancellationToken)
        {
            var buffer = new byte[8192];
            long totalNumberBytesRead = 0;

            _totalStreamLength = 0L;
            _audioQueueTimeline = null;

            Console.WriteLine($"Downloader started");
            try
            {
                using (_player = new StreamingPlayback())
                {
                    _player.OutputReady += OnPlayerOutputReady;
                    _player.Finished += OnPlayerFinished;
                    _player.AudioPlayerStateChanged += OnAudioPlayerStateChanged;

                    Uri requestUri = GetRequestUri(guid);
                    using (var httpClient = await _requestService.GetHttpClient())
                    {
                        using (var response = await TryGetAsync(0, requestUri, httpClient, cancellationToken))
                        {
                            _totalStreamLength = response.Content.Headers.ContentLength.GetValueOrDefault(totalNumberBytesRead);
                            Console.WriteLine($"Stream Length: {_totalStreamLength}");

                            using (var inputStream = await GetQueueStream(response.Content, cancellationToken))
                            {
                                //var inputStream = await response.Content.ReadAsStreamAsync();
                                int inputStreamLength;

                                while (((inputStreamLength = inputStream.Read(buffer, 0, buffer.Length)) != 0) || !cancellationToken.IsCancellationRequested)
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        cancellationToken.ThrowIfCancellationRequested();
                                    }
                                    //Console.WriteLine($"{nameof(StreamDownloadHandler)} read {totalNumberBytesRead} from {_totalStreamLength} ");
                                    totalNumberBytesRead += inputStreamLength;
                                    _player.ParseBytes(buffer, inputStreamLength, false, totalNumberBytesRead == (int)_totalStreamLength);
                                }
                            }
                        }
                    }
                }
            }
            catch (HttpStatusRequestException exception)
            {
                Console.WriteLine($"Exception thrown in {nameof(StreamDownloadHandler)} with message {exception.Message}");
                if (exception.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MediaStateChanged(MediaState.BadRequest);
                }
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
                throw new HttpStatusRequestException($"{nameof(TryGetAsync)} aborted after 5 attempts", responseMessage.StatusCode);
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

        private async Task<Stream> GetQueueStream(HttpContent content, CancellationToken cancellationToken)
        {
            var queueStream = new QueueStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/copy.mp3");
            var inputStream = await content.ReadAsStreamAsync();
            {
                var t = Task.Run(() =>
                {
                    var buffer = new byte[8192];
                    int count = 0;

                    while ((count = inputStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        //Console.WriteLine($"{nameof(GetQueueStream)} length {queueStream.Length} ");
                        queueStream.Push(buffer, 0, count);
                    }
                }, cancellationToken);
            }
            return queueStream;
        }

        
    }
}