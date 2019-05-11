using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ExtendedSplashPageViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;
        private readonly ITunesService tunesService;
        private readonly IAuthenticationService authenticationService;

        public ExtendedSplashPageViewModel(INavigationService navigationService,
            ISettingsService settingsService,
            ITunesService tunesService,
            IAuthenticationService authenticationService) : base(navigationService)
        {
            this.settingsService = settingsService;
            this.tunesService = tunesService;
            this.authenticationService = authenticationService;
        }

        public async override void OnNavigatingTo(INavigationParameters parameters)
        {
            try
            {
                var isAccessible = await this.tunesService.IsEndPointAccessibleAsync(this.settingsService.ServiceEndPoint);
                {
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
            }
            catch (Exception)
            {
                await NavigationService.NavigateAsync("ServiceEndpointSettingsPage");
            }
        }
    }
}
