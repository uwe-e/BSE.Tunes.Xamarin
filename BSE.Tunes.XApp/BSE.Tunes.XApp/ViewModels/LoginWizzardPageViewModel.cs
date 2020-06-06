using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;

namespace BSE.Tunes.XApp.ViewModels
{
    public class LoginWizzardPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IAuthenticationService _authenticationService;
        private DelegateCommand _saveCommand;
        private string _userName;
        private string _password;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                SetProperty(ref _userName, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SetProperty(ref _password, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save, CanSave));

        public LoginWizzardPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IPageDialogService pageDialogService,
            IAuthenticationService authenticationService) : base(navigationService, resourceService)
        {
            _pageDialogService = pageDialogService;
            _authenticationService = authenticationService;
        }

        private bool CanSave()
        {
            return !String.IsNullOrEmpty(UserName) && !(string.IsNullOrEmpty(Password));
        }

        private async void Save()
        {
            try
            {
                await _authenticationService.LoginAsync(UserName, Password);
                await NavigationService.NavigateAsync(nameof(MainPage));
            }
            catch (Exception)
            {
                var message = ResourceService.GetString("LoginPageViewModel_LoginException");
                var dialogResult = ResourceService.GetString("Dialog_Result_Ok");
                await _pageDialogService.DisplayAlertAsync("", message, dialogResult);
            }
        }

    }
}
