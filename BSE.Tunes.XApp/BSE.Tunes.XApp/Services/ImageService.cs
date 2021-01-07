﻿using BSE.Tunes.XApp.Events;
using Prism.Events;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public class ImageService : IImageService
    {
        private const string ThumbnailPart = "_thumb";
        private const string ImageExtension = "img";
        private readonly IStorageService _storageService;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRequestService _requestService;


        public ImageService(
            IStorageService storageService,
			IDataService dataService,
			ISettingsService settingsService,
			IEventAggregator eventAggregator,
            IRequestService requestService)
        {
            _storageService = storageService;
            _dataService = dataService;
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
            _requestService = requestService;
        }

		public string GetBitmapSource(Guid albumId, bool asThumbnail = false)
        {
			string fileName = asThumbnail ? $"{albumId}_{ThumbnailPart}" : $"{albumId}";
			fileName = $"{fileName}.{ImageExtension}";


            if (_storageService.TryToGetImagePath(fileName, out string fileFullName))
            {
                return fileFullName;
            }

            string absoluteUri = GetImageUrl(asThumbnail, albumId).AbsoluteUri;
            
            //Fire and forget
            Task.Run(() =>
            {
                //we create and save the image into the file system to use it next time.
                CreateAndSaveBitmap(absoluteUri, fileFullName, asThumbnail);
            }).ConfigureAwait(false);
                
            return absoluteUri;
        }
        
        public async Task RemoveStitchedBitmaps(int playlistId)
        {
            string searchPattern = $"{playlistId}_*.{ImageExtension}";
            await _storageService.DeleteCachedImagesAsync(searchPattern);
        }

        public async Task<string> GetStitchedBitmapSource(int playlistId, int width = 300, bool asThumbnail = false)
		{
			if (playlistId > 0)
            {
				string fileName = $"{playlistId}_{width}.{ImageExtension}";
				if (!_storageService.TryToGetImagePath(fileName, out string fullName))
                {
					int height = width;

                    ObservableCollection<Guid> albumIds = await GetImageIds(playlistId);

                    SKImage stitchedImage = await Combine(albumIds, width, height, asThumbnail);

                    using (SKData encoded = stitchedImage.Encode(SKEncodedImageFormat.Png, 100))
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

        private async void CreateAndSaveBitmap(string imageUri, string fileName, bool asThumbnail)
        {
            SKBitmap bitmap = await CreateBitmapFromStream(imageUri);
            if (bitmap != null)
            {
                if (!asThumbnail)
                {
                    bitmap = bitmap.Resize(new SKImageInfo(300, 300), SKFilterQuality.Medium);
                }
                using (SKImage image = SKImage.FromBitmap(bitmap))
                {
                    using (SKData encoded = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                    {
                        using (System.IO.Stream outFile = System.IO.File.OpenWrite(fileName))
                        {
                            encoded.SaveTo(outFile);
                        }
                    }
                    _eventAggregator.GetEvent<CacheChangedEvent>().Publish(CacheChangeMode.Added);
                }
            }
        }

        private async Task<ObservableCollection<Guid>> GetImageIds(int playlistId)
        {
            return await _dataService.GetPlaylistImageIdsById(playlistId, _settingsService.User.UserName, 4);
        }

        public async Task<string> GetStitchedBitmapSource(IEnumerable<Guid> albumIds, string cacheName, int width, bool asThumbnail = false)
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
					string imageUri = GetImageUrl(asThumbnail, id).AbsoluteUri;
					if (imageUri != null)
					{
						var bitmap = await CreateBitmapFromStream(imageUri);
						if (bitmap != null)
						{
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

                    if (images.Count == 1)
                    {
                        canvas.DrawBitmap(images[0], SKRect.Create(0, 0, width, height));
                    }
                    else
                    {
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

        private async Task<SKBitmap> CreateBitmapFromStream(string imageUri)
        {
            SKBitmap bitmap = default;

            if (imageUri != null)
            {
                using (var httpClient = await _requestService.GetHttpClient())
                {
                    try
                    {
                        var stream = await httpClient.GetStreamAsync(imageUri);
                        if (stream != null)
                        {
                            //create a bitmap from the file and add it to the list
                            bitmap = SKBitmap.Decode(stream);
                        }
                    }
                    //if there´s no image
                    catch (Exception) { }
                }
            }

            return bitmap;
        }

        private Uri GetImageUrl(bool asThumbnail, Guid id)
        {
            return _dataService.GetImage(id, asThumbnail);
        }

        

        
    }
}
