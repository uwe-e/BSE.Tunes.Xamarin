using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ServiceEndpointSettingsPageViewModel : BaseSettingsPageViewModel
    {
        private readonly IPageDialogService _pageDialogService;
        private string _serviceEndPoint;

        public string ServiceEndPoint
        {
            get
            {
                return _serviceEndPoint;
            }
            set
            {
                SetProperty(ref _serviceEndPoint, value);
            }
        }

        public ServiceEndpointSettingsPageViewModel(
            INavigationService navigationService,
            ISettingsService settingsService,
            IPageDialogService pageDialogService,
            IPlayerManager playerManager,
            IResourceService resourceService) : base(navigationService, settingsService, playerManager, resourceService)
        {
            _pageDialogService = pageDialogService;
        }

        public override void LoadSettings()
        {
            ServiceEndPoint = SettingsService?.ServiceEndPoint;
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
                ResourceService.GetString("ServiceEndpointSettingsPage_ActionSheet_Title"),
                buttons);
        }

        private async void DeleteAction()
        {
            if (await PlayerManager.CloseAsync())
            {
                SettingsService.ServiceEndPoint = null;

                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(ServiceEndpointWizzardPage)}");
            }
        }

    }
}
