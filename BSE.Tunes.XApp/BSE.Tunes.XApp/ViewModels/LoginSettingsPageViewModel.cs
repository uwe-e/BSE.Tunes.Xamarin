using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.ViewModels
{
    public class LoginSettingsPageViewModel : BaseSettingsPageViewModel
    {
        private string _userName;
        private readonly IPageDialogService _pageDialogService;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                SetProperty(ref _userName, value);
            }
        }

        public LoginSettingsPageViewModel(INavigationService navigationService,
            ISettingsService settingsService,
            IPlayerManager playerManager,
            IResourceService resourceService,
            IPageDialogService pageDialogService) : base(navigationService, settingsService, playerManager, resourceService)
        {
            _pageDialogService = pageDialogService;
        }

        public override void LoadSettings()
        {
            UserName = SettingsService?.User.UserName;
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
                ResourceService.GetString("LoginSettingsPage_ActionSheet_Title"),
                buttons);
        }

        private async void DeleteAction()
        {
            if (await PlayerManager.CloseAsync())
            {
                SettingsService.User = null;

                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(LoginWizzardPage)}");
            }
        }
    }
}
