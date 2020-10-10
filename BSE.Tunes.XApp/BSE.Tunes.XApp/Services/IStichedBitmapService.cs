using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IStichedBitmapService
    {
        Task<string> GetBitmapSource(int playlistId, int width = 300, bool asThumbnail = false);
        Task<string> GetBitmapSource(IEnumerable<Guid> albumIds, string cacheName, int width, bool asThumbnail = false);
    }
}
