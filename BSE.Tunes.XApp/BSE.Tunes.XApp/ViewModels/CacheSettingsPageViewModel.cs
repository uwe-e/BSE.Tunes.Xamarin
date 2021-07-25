using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using BSE.Tunes.XApp.Views.Settings;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;

namespace BSE.Tunes.XApp.ViewModels
{
    public class CacheSettingsPageViewModel : BaseSettingsPageViewModel
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IStorageService _storageService;
        private readonly IEventAggregator _eventAggregator;
        private string _usedDiskSpace;
        private bool _isCacheChanged;

        public string UsedDiskSpace
        {
            get
            {
                return _usedDiskSpace;
            }
            set
            {
                SetProperty(ref _usedDiskSpace, value);
            }
        }

        public CacheSettingsPageViewModel(
            INavigationService navigationService,
            ISettingsService settingsService,
            IPlayerManager playerManager,
            IPageDialogService pageDialogService,
            IStorageService storageService,
            IEventAggregator eventAggregator,
            IResourceService resourceService) : base(navigationService, settingsService, playerManager, resourceService)
        {
            _pageDialogService = pageDialogService;
            _storageService = storageService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<CacheChangedEvent>().Subscribe((args) =>
            {
                LoadSettings();
            });

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (uniqueTrack) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(CacheSettingsPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", uniqueTrack.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
        }

        public async override void LoadSettings()
        {
            if (!_isCacheChanged)
            {
                _isCacheChanged = true;

                var usedSpace = await _storageService.GetUsedDiskSpaceAsync();
                UsedDiskSpace = $"{Math.Round(Convert.ToDecimal(usedSpace / 1024f / 1024f), 2)} MB";
                
                _isCacheChanged = false;
            }
        }

        public async override void DeleteSettings()
        {
            var buttons = new IActionSheetButton[]
            {
                ActionSheetButton.CreateCancelButton(
                    ResourceService.GetString("ActionSheetButton_Cancel")),
                ActionSheetButton.CreateDestroyButton(
                    ResourceService.GetString("ActionSheetButton_Delete"),
                    DeleteAction)
            };

            await _pageDialogService.DisplayActionSheetAsync(
                ResourceService.GetString("CacheSettingsPage_ActionSheet_Title"),
                buttons);
        }
         
        private async void DeleteAction()
        {
            await _storageService.DeleteCachedImagesAsync();

            _eventAggregator.GetEvent<CacheChangedEvent>().Publish(CacheChangeMode.Removed);
        }
    }
}
