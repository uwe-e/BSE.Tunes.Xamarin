using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using Prism.Navigation;
using System;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ExtendedSplashPageViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;
        private readonly IDataService dataService;
        private readonly IAuthenticationService authenticationService;

        public ExtendedSplashPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            ISettingsService settingsService,
            IDataService dataService,
            IAuthenticationService authenticationService) : base(navigationService, resourceService)
        {
            this.settingsService = settingsService;
            this.dataService = dataService;
            this.authenticationService = authenticationService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                var isAccessible = await this.dataService.IsEndPointAccessibleAsync(this.settingsService.ServiceEndPoint);
                {
                    if (this.settingsService.User is User user)
                    {
                        try
                        {
                            await this.authenticationService.RequestRefreshTokenAsync(user.Token);
                            //await NavigationService.NavigateAsync("MainPage/NavigationPage/HomePage");
                            await NavigationService.NavigateAsync("/MainPage/HomePage");
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
            }
            catch (Exception)
            {
                await NavigationService.NavigateAsync("ServiceEndpointSettingsPage");
            }
        }
    }
}
