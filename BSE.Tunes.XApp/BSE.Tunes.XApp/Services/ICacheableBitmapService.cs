using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface ICacheableBitmapService
    {
        Task<string> GetBitmapSource(IEnumerable<Guid> albumIds, string cacheName, int width, bool asThumbnail = false);
    }
}
