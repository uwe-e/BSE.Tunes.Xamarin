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

        public bool TryToGetImagePath(string fileName, out string filePath)
        {
            var imageFolderPath = GetImageFolder();
            filePath = Path.Combine(imageFolderPath, fileName);
            return File.Exists(filePath);
        }
    }
}
