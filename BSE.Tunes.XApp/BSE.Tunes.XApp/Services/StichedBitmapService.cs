using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public class StichedBitmapService : IStichedBitmapService
    {
        private const string ThumbnailPart = "_thumb";
        private const string ImageExtension = "png";
        private readonly IStorageService _storageService;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IRequestService _requestService;


        public StichedBitmapService(
            IStorageService storageService,
			IDataService dataService,
			ISettingsService settingsService,
            IRequestService requestService)
        {
            _storageService = storageService;
            _dataService = dataService;
            _settingsService = settingsService;
            _requestService = requestService;
        }
		public async Task<string> GetBitmapSource(int playlistId, int width = 300, bool asThumbnail = false)
		{
			if (playlistId > 0)
            {
				string fileName = $"{playlistId}_{width}.{ImageExtension}";
				if (!_storageService.TryToGetImagePath(fileName, out string fullName))
                {
					int height = width;

                    ObservableCollection<Guid> albumIds = await GetImageIds(playlistId);

                    SKImage stitchedImage = await Combine(albumIds, width, height, asThumbnail);

                    using (SKData encoded = stitchedImage.Encode(SKEncodedImageFormat.Png, 150))
                    {
                        using (System.IO.Stream outFile = System.IO.File.OpenWrite(fullName))
                        {
                            encoded.SaveTo(outFile);
                        }
                    }
                }
				return fullName;
			}
			return null;
		}

        private async Task<ObservableCollection<Guid>> GetImageIds(int playlistId)
        {
            return await _dataService.GetPlaylistImageIdsById(playlistId, _settingsService.User.UserName, 4);
        }

        public async Task<string> GetBitmapSource(IEnumerable<Guid> albumIds, string cacheName, int width, bool asThumbnail = false)
        {
			if (albumIds != null)
			{
				cacheName = asThumbnail ? cacheName + ThumbnailPart : cacheName;
				var height = width;

				string fileName = $"{cacheName}.{ImageExtension}";
                if (!_storageService.TryToGetImagePath(fileName, out string fullName))
				{
					SKImage stitchedImage = await Combine(albumIds, width, height, asThumbnail);

					using (SKData encoded = stitchedImage.Encode(SKEncodedImageFormat.Png, 150))
					{
						using (System.IO.Stream outFile = System.IO.File.OpenWrite(fullName))
						{
							encoded.SaveTo(outFile);
						}
					}
				}
				return fullName;
			}
			return null;
        }

		public async Task<SKImage> Combine(IEnumerable<Guid> albumIds, int width, int height, bool asThumbnail = false)
		{
			//read all images into memory
			List<SKBitmap> images = new List<SKBitmap>();
			SKImage finalImage = null;

			try
			{
				foreach (var id in albumIds)
				{
					var uri = _dataService.GetImage(id, asThumbnail);

					using (var httpClient = await _requestService.GetHttpClient())
                    {
						var stream = await httpClient.GetStreamAsync(uri);
						if (stream != null)
						{
							//create a bitmap from the file and add it to the list
							SKBitmap bitmap = SKBitmap.Decode(stream);
							images.Add(bitmap);
						}
                    }
				}

				//get a surface so we can draw an image
				using (var tempSurface = SKSurface.Create(new SKImageInfo(width, height)))
				{
					//get the drawing canvas of the surface
					var canvas = tempSurface.Canvas;

					//set background color
					canvas.Clear(SKColors.Transparent);

					var innerWidth = width / 2;
					var innerHeight = innerWidth;
					int index = 0;

					foreach (SKBitmap image in images)
					{
						int x = 0;
						int y = 0;

						if (index == 1 || index == 2)
						{
							x += innerWidth;
						}
						if (index == 1 || index == 3)
						{
							y += innerHeight;
						}

						canvas.DrawBitmap(image, SKRect.Create(x, y, innerWidth, innerHeight));
						index++;
					}

					// return the surface as a manageable image
					finalImage = tempSurface.Snapshot();
				}

				//return the image that was just drawn
				return finalImage;
			}
			finally
			{
				//clean up memory
				foreach (SKBitmap image in images)
				{
					image.Dispose();
				}
			}
		}

        
    }
}
