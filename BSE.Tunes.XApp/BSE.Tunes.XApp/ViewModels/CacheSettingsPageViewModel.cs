using BSE.Tunes.XApp.Services;
using Prism.Navigation;
using Prism.Services;
using System;

namespace BSE.Tunes.XApp.ViewModels
{
    public class CacheSettingsPageViewModel : BaseSettingsPageViewModel
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IStorageService _storageService;
        private string _usedDiskSpace;

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
            IResourceService resourceService) : base(navigationService, settingsService, playerManager, resourceService)
        {
            _pageDialogService = pageDialogService;
            _storageService = storageService;
        }

        public async override void LoadSettings()
        {
            var usedSpace = await _storageService.GetUsedDiskSpaceAsync();
            if (usedSpace > 0)
            {
                UsedDiskSpace = $"{Math.Round(Convert.ToDecimal(usedSpace / 1024f / 1024f), 2)} MB";
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
        }
    }
}
