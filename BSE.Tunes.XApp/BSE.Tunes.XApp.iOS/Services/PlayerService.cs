using AVFoundation;
using BSE.Tunes.XApp.iOS.Services;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Foundation;
using System;
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
        private IRequestService _requestService => DependencyService.Resolve<IRequestService>();
        private ISettingsService _settingsService => DependencyService.Resolve<ISettingsService>();
        private StreamingPlayback _player;

        public void Pause()
        {
        }

        public void Play()
        {
        }

        public void PlayNext()
        {
        }
        public async Task SetTrackAsync(Track track)
        {
            if (track != null)
            {
                Guid guid = track.Guid;
                //guid = new Guid("f856d83e-cf83-425f-b2e2-254ad704e766");
                // Create a shared intance session & check
                using (var session = AVAudioSession.SharedInstance())
                {
                    if (session != null)
                    {
                        await Task.Run(() => StreamDownloadHandler(guid));
                        // Set up the session for playback category
                        NSError error;
                        session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.DefaultToSpeaker);
                        session.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out error);
                    }
                }
            }
        }

        private async Task StreamDownloadHandler(Guid guid)
        {
            var buffer = new byte[8192];
            long totalNumberBytesRead = -1;
            int bytesReceived = 0;
            double sampleRate = 0;

            try
            {
                using (_player = new StreamingPlayback())
                {
                    _player.OutputReady += delegate
                    {
                        //timeline = player.OutputQueue.CreateTimeline();
                        sampleRate = _player.OutputQueue.SampleRate;
                    };
                    _player.Finished += delegate
                    {

                    };

                    Uri requestUri = GetRequestUri(guid);
                    using (var httpClient = await _requestService.GetHttpClient(true))
                    {
                        using (var response = await httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None))
                        {
                            long totalStreamLength = response.Content.Headers.ContentLength.GetValueOrDefault(totalNumberBytesRead);

                            using (var inputStream = await response.Content.ReadAsStreamAsync())
                            {
                                do
                                {
                                    if (bytesReceived < totalStreamLength)
                                    {
                                        bytesReceived = await inputStream.ReadAsync(buffer, 0, buffer.Length, CancellationToken.None);
                                        //Console.WriteLine($"{ totalNumberBytesRead  } + {bytesReceived } from { totalStreamLength}");
                                        _player.ParseBytes(buffer, bytesReceived, false, totalNumberBytesRead == (int)totalStreamLength);
                                        totalNumberBytesRead += bytesReceived;
                                    }
                                }
                                while (totalNumberBytesRead != totalStreamLength);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
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