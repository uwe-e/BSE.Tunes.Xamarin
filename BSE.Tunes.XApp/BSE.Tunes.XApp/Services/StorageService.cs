using FFImageLoading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BSE.Tunes.XApp.Services
{
    public class StorageService : IStorageService
    {
        private const string CacheFolderName = "cache";
        private const string ImageFolderName = "img";

        public Task<DirectoryInfo> GetCacheFolderAsync()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cacheFolder = Path.Combine(folder, CacheFolderName);
            if (!Directory.Exists(cacheFolder))
            {
                Directory.CreateDirectory(cacheFolder);
            }

            return null;
        }

        public bool TryToGetImagePath(string fileName, out string filePath)
        {
            var imageFolderPath = GetImageFolder();
            filePath = Path.Combine(imageFolderPath, fileName);
            return File.Exists(filePath);
        }

        public Task<long> GetUsedDiskSpaceAsync()
        {
            return Task.Run(() =>
            {
                return GetUsedDiskSpace();
            });
        }

        public long GetUsedDiskSpace()
        {
            long length = default;
            string imageFolderPath = GetImageFolder();
            DirectoryInfo directoryInfo = new DirectoryInfo(imageFolderPath);
            if (directoryInfo.Exists)
            {
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    length += fileInfo.Length;
                }
            }
            return length;
        }

        public string GetImageFolder()
        {
            var cacheFolder = FileSystem.CacheDirectory;
            var imageFolderPath = Path.Combine(cacheFolder, ImageFolderName);
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }
            return imageFolderPath;
        }

        public Task DeleteCachedImagesAsync()
        {
            string imageFolderPath = GetImageFolder();
            DirectoryInfo directoryInfo = new DirectoryInfo(imageFolderPath);
            if (directoryInfo.Exists)
            {
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    fileInfo.Delete();
                }
            }
            return Task.CompletedTask;
        }
    }
}
