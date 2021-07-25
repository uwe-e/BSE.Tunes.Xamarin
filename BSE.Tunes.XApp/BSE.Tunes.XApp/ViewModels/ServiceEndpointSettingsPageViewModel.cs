using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ServiceEndpointSettingsPageViewModel : BaseSettingsPageViewModel
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IEventAggregator _eventAggregator;
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
            IEventAggregator eventAggregator,
            IResourceService resourceService) : base(navigationService, settingsService, playerManager, resourceService)
        {
            _pageDialogService = pageDialogService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (uniqueTrack) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(ServiceEndpointSettingsPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", uniqueTrack.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
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
