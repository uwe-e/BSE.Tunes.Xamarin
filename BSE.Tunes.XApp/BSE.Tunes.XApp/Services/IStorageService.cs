using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IStorageService
    {
        string GetImageFolder();

        bool TryToGetImagePath(string fileName, out string filePath);

        Task<long> GetUsedDiskSpaceAsync();

        long GetUsedDiskSpace();

        Task DeleteCachedImagesAsync(string searchPattern = null);
    }
}
