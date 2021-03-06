﻿using System;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ServiceEndpointSettingsWizzardViewModel : ViewModelBase
    {
        private string _serviceEndPoint;
        private DelegateCommand _saveCommand;
        private readonly IPageDialogService _pageDialogService;
        private readonly ISettingsService _settingsService;
        private readonly IDataService _dataService;
        private readonly IAuthenticationService _authenticationService;

        public string ServiceEndPoint
        {
            get => _serviceEndPoint;
            set
            {
                SetProperty(ref _serviceEndPoint, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save, CanSave));

        public ServiceEndpointSettingsWizzardViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IPageDialogService pageDialogService,
            ISettingsService settingsService,
            IDataService dataService,
            IAuthenticationService authenticationService) : base(navigationService, resourceService)
        {
            _pageDialogService = pageDialogService;
            _settingsService = settingsService;
            _dataService = dataService;
            _authenticationService = authenticationService;
        }
        
        private bool CanSave()
        {
            return !String.IsNullOrEmpty(ServiceEndPoint);
        }
        
        private async void Save()
        {
            var uriBuilder = new UriBuilder(ServiceEndPoint);
            var serviceEndPoint = uriBuilder.Uri.AbsoluteUri;
            try
            {
                await _dataService.IsEndPointAccessibleAsync(serviceEndPoint);
                _settingsService.ServiceEndPoint = serviceEndPoint;
                if (_settingsService.User is User user)
                {
                    try
                    {
                        await this._authenticationService.RequestRefreshTokenAsync(user.Token);
                        await NavigationService.NavigateAsync(nameof(MainPage));
                    }
                    catch (Exception)
                    {
                        await NavigationService.NavigateAsync(nameof(LoginWizzardPage));
                    }
                }
                else
                {
                    await NavigationService.NavigateAsync(nameof(LoginWizzardPage));
                }
            }
            catch (Exception exception)
            {
                var title = ResourceService.GetString("AlertDialog_Error_Title_Text");
                var dialogResult = ResourceService.GetString("Dialog_Result_Cancel");

                await _pageDialogService.DisplayAlertAsync(
                    title,
                    exception.Message,
                    dialogResult);
            }
        }
    }
}
