using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IImageService
    {
        string GetBitmapSource(Guid albumId, bool asThumbnail = false);
        Task<string> GetStitchedBitmapSource(int playlistId, int width = 300, bool asThumbnail = false);
        Task RemoveStitchedBitmaps(int playlistId);
        Task<string> GetStitchedBitmapSource(IEnumerable<Guid> albumIds, string cacheName, int width, bool asThumbnail = false);
    }
}
