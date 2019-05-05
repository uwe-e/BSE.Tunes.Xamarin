using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService pageDialogService;
        private readonly ISettingsService settingsService;
        private readonly IAuthenticationService authenticationService;
        private string userName;
        private DelegateCommand saveCommand;
        private string password;

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                SetProperty(ref userName, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                SetProperty(ref password, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand => saveCommand ?? (saveCommand = new DelegateCommand(Save, CanSave));

        public LoginPageViewModel(INavigationService navigationService,
            IPageDialogService pageDialogService,
            ISettingsService settingsService,
            IAuthenticationService authenticationService) : base(navigationService)
        {
            this.pageDialogService = pageDialogService;
            this.settingsService = settingsService;
            this.authenticationService = authenticationService;
        }

        private bool CanSave()
        {
            return !String.IsNullOrEmpty(UserName) && !(string.IsNullOrEmpty(Password));
        }

        private async void Save()
        {
            try
            {
                await authenticationService.LoginAsync(UserName, Password);
                await NavigationService.NavigateAsync("MainPage");
            }
            catch (Exception exception)
            {
                await this.pageDialogService.DisplayAlertAsync("test", exception.Message, "cancel");
            }
        }

    }
}
