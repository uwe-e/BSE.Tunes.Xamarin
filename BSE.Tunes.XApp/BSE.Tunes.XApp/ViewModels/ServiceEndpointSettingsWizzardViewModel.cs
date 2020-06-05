using System;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ServiceEndpointSettingsWizzardViewModel : ViewModelBase
    {
        private string serviceEndPoint;
        private DelegateCommand saveCommand;
        private readonly IPageDialogService pageDialogService;
        private readonly ISettingsService settingsService;
        private readonly IDataService dataService;
        private readonly IAuthenticationService authenticationService;

        public string ServiceEndPoint
        {
            get
            {
                return this.serviceEndPoint;
            }
            set
            {
                SetProperty(ref this.serviceEndPoint, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand => this.saveCommand ?? (this.saveCommand = new DelegateCommand(Save, CanSave));

        private bool CanSave()
        {
            return !String.IsNullOrEmpty(ServiceEndPoint);
        }

        public ServiceEndpointSettingsWizzardViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IPageDialogService pageDialogService,
            ISettingsService settingsService,
            IDataService dataService,
            IAuthenticationService authenticationService) : base(navigationService, resourceService)
        {
            this.pageDialogService = pageDialogService;
            this.settingsService = settingsService;
            this.dataService = dataService;
            this.authenticationService = authenticationService;
        }

        private async void Save()
        {
            var uriBuilder = new UriBuilder(ServiceEndPoint);
            var serviceEndPoint = uriBuilder.Uri.AbsoluteUri;
            try
            {
                await this.dataService.IsEndPointAccessibleAsync(serviceEndPoint);
                this.settingsService.ServiceEndPoint = serviceEndPoint;
                if (this.settingsService.User is User user)
                {
                    try
                    {
                        await this.authenticationService.RequestRefreshTokenAsync(user.Token);
                        await NavigationService.NavigateAsync("MainPage/NavigationPage/HomePage");
                    }
                    catch (Exception)
                    {
                        await NavigationService.NavigateAsync("LoginPage");
                    }
                }
                else
                {
                    await NavigationService.NavigateAsync("LoginPage");
                }
            }
            catch (Exception)
            {
                await this.pageDialogService.DisplayAlertAsync("test", "message", "cancel");
            }
        }
    }
}
