using BSE.Tunes.XApp.iOS.Services;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;

[assembly: Dependency(typeof(PlayerService))]
namespace BSE.Tunes.XApp.iOS.Services
{
    public class PlayerService : IPlayerService
    {
        long _totalBytesToReceive;

        IStreamingService _streamingService => DependencyService.Resolve<IStreamingService>();
        IRequestService _requestService => DependencyService.Resolve<IRequestService>();

        ISettingsService _settingsService => DependencyService.Resolve<ISettingsService>();

        StreamingPlayback _player;

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
                var buffer = new byte[8192];
                long totalNumberBytesRead = 0;
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
                                long totalStreamLength = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);

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
                        //do
                        //{
                        //    if (bytesReceived < totalBytesToReceive)
                        //    {
                        //        l += bytesReceived;
                        //        //        //int receivedBytes = responseStream.Read(buffer, 0, buffer.Length);
                        //        //        //player.ParseBytes(buffer, receivedBytes, false, bytesReceived == (int)totalBytesToReceive);
                        //        //        //bytesReceived += (long)receivedBytes;

                        //        long newTo = bytesReceived + (buffer.Length - 1);
                        //        Console.WriteLine($"{ bytesReceived } + {newTo} from { totalBytesToReceive}");
                        //        var stream = await _streamingService.GetPartialStream(guid, bytesReceived, newTo);
                        //        //int receivedBytes = stream.Read(buffer, 0, buffer.Length);
                        //        int receivedBytes = stream.Read(buffer, 0, (int)stream.Length);

                        //        player.ParseBytes(buffer, receivedBytes, false, l == (int)totalBytesToReceive);

                        //        bytesReceived += (long)receivedBytes;
                        //    }
                        //}
                        //while (bytesReceived != totalBytesToReceive);

                        //do
                        //{
                        //    if (bytesReceived < totalBytesToReceive)
                        //    {
                        //        l += bytesReceived;
                        //        long newTo = bytesReceived + (buffer.Length - 1);
                        //        var stream = await _streamingService.GetPartialStream(guid, bytesReceived, newTo);
                        //        if (queueStream == null)
                        //        {
                        //            queueStream = new QueueStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/copy.mp3");
                        //        }
                        //        else
                        //        {
                        //            var t = new Thread((x) => {
                        //                //var tbuf = new byte[8192];
                        //                int count;

                        //                while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
                        //                    queueStream.Push(buffer, 0, count);
                        //                //stream.CopyToAsync(queueStream, buffer.Length);
                        //                //queueStream.Push()
                        //            });
                        //            t.Start();
                        //            bytesReceived += (long)buffer.Length;
                        //        }

                        //    }
                        //}
                        //while (bytesReceived != totalBytesToReceive);


                        //var responseStream = await _streamingService.GetStream(guid);
                        //System.IO.Stream inputStream = responseStream;
                        ////System.IO.Stream responseStream = null; 
                        //while ((inputStreamLength = inputStream.Read(buffer, 0, buffer.Length)) != 0 && player != null)
                        //{
                        //    l += inputStreamLength;
                        //    Console.WriteLine($"{ inputStreamLength } + {buffer.Length} from { totalBytesToReceive}");
                        //}

                        //while ((inputStreamLength = inputStream.Read(buffer, 0, buffer.Length)) != 0 && player != null)
                        //{
                        //    l += inputStreamLength;
                        //    Console.WriteLine($"{ inputStreamLength } + {buffer.Length} from { totalBytesToReceive}");

                        //    player.ParseBytes(buffer, inputStreamLength, false, l == (int)totalBytesToReceive);

                        //    //InvokeOnMainThread(delegate {
                        //    //    progressBar.Progress = l / (float)response.ContentLength;
                        //    //});
                        //}
                    }
                }
                catch (Exception exception)
                {
                    var msg = exception.Message;
                }
            }
        }

        private Uri GetRequestUri(Guid guid)
        {
            var builder = new UriBuilder(_settingsService.ServiceEndPoint);
            builder.Path = Path.Combine(builder.Path, $"/api/files/audio/{guid}");
            return builder.Uri;
        }


        Stream GetQueueStream(Stream responseStream)
        {
            var queueStream = new QueueStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/copy.mp3");
            var t = new Thread((x) => {
                var tbuf = new byte[8192];
                int count;

                while ((count = responseStream.Read(tbuf, 0, tbuf.Length)) != 0)
                {
                    Console.WriteLine($"queueStream {count}");
                    queueStream.Push(tbuf, 0, count);
                }
                    

            });
            t.Start();
            return queueStream;
        }
    }
}