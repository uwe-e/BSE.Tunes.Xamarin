using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Navigation;
using System;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ExtendedSplashPageViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IDataService _dataService;
        private readonly IAuthenticationService _authenticationService;

        public ExtendedSplashPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            ISettingsService settingsService,
            IDataService dataService,
            IAuthenticationService authenticationService) : base(navigationService, resourceService)
        {
            _settingsService = settingsService;
            _dataService = dataService;
            _authenticationService = authenticationService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                var isAccessible = await _dataService.IsEndPointAccessibleAsync(_settingsService.ServiceEndPoint);
                {
                    if (_settingsService.User is User user)
                    {
                        try
                        {
                            await _authenticationService.RequestRefreshTokenAsync(user.Token);
                            await NavigationService.NavigateAsync("MainPage");
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
                await NavigationService.NavigateAsync(nameof(ServiceEndpointWizzardPage));
            }
        }
    }
}
