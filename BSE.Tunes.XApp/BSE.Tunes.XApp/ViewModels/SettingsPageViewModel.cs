using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using BSE.Tunes.XApp.Views.Settings;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase, IActiveAware
    {
        private readonly ISettingsService _settingsService;
        private readonly IStorageService _storageService;
        private bool _isActive;
        private bool _isActivated;
        private string _serviceEndPoint;
        private string _userName;
        private string _usedDiskSpace;
        private DelegateCommand _toServiceEndpointDetailCommand;
        private DelegateCommand _toAccountDetailCommand;
        private ICommand _toCacheSettingsDetailCommand;

        public ICommand ToServiceEndpointDetailCommand
            => _toServiceEndpointDetailCommand ?? (_toServiceEndpointDetailCommand = new DelegateCommand(NavigateToServiceEndpointDetail));

        public ICommand ToAccountDetailCommand
           => _toAccountDetailCommand ?? (_toAccountDetailCommand = new DelegateCommand(NavigateToAccountDetail));

        public ICommand ToCacheSettingsDetailCommand => _toCacheSettingsDetailCommand ?? (_toCacheSettingsDetailCommand = new DelegateCommand(NavigateToCacheSettingsDetail));

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }

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

        public string UsedDiskSpace
        {
            get
            {
                return _usedDiskSpace;
            }
            set
            {
                SetProperty(ref _usedDiskSpace, value);
            }
        }

        public event EventHandler IsActiveChanged;

        public SettingsPageViewModel(INavigationService navigationService,
            ISettingsService settingsService,
            IResourceService resourceService,
            IStorageService storageService) : base(navigationService, resourceService)
        {
            _settingsService = settingsService;
            _storageService = storageService;
        }

        private void RaiseIsActiveChanged()
        {
            if (IsActive && !_isActivated)
            {
                _isActivated = true;

                LoadSettings();
            }
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void LoadSettings()
        {
            ServiceEndPoint = _settingsService?.ServiceEndPoint;
            UserName = _settingsService?.User.UserName;
            var usedSpace = await _storageService.GetUsedDiskSpaceAsync();
            if (usedSpace > 0)
            {
                UsedDiskSpace = $"{Math.Round(Convert.ToDecimal(usedSpace / 1024f / 1024f),2)} MB";
            }
        }
        
        private async void NavigateToAccountDetail()
        {
            await NavigationService.NavigateAsync(nameof(LoginSettingsPage));
        }

        private async void NavigateToServiceEndpointDetail()
        {
            await NavigationService.NavigateAsync(nameof(ServiceEndpointSettingsPage));
        }
        
        private async void NavigateToCacheSettingsDetail()
        {
            await NavigationService.NavigateAsync(nameof(CacheSettingsPage));
        }
    }
}
