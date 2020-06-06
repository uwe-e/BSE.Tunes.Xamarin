using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
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
        private bool _isActive;
        private bool _isActivated;
        private string _serviceEndPoint;
        private string _userName;
        private DelegateCommand _toServiceEndpointDetailCommand;
        private DelegateCommand _toAccountDetailCommand;

        public ICommand ToServiceEndpointDetailCommand
            => _toServiceEndpointDetailCommand ?? (_toServiceEndpointDetailCommand = new DelegateCommand(NavigateToServiceEndpointDetail));

        public ICommand ToAccountDetailCommand
           => _toAccountDetailCommand ?? (_toAccountDetailCommand = new DelegateCommand(NavigateToAccountDetail));

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

        public event EventHandler IsActiveChanged;

        public SettingsPageViewModel(INavigationService navigationService,
            ISettingsService settingsService,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
            _settingsService = settingsService;
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

        private void LoadSettings()
        {
            ServiceEndPoint = _settingsService?.ServiceEndPoint;
            UserName = _settingsService?.User.UserName;
        }
        
        private async void NavigateToAccountDetail()
        {
            await NavigationService.NavigateAsync(nameof(LoginSettingsPage));
        }

        private async void NavigateToServiceEndpointDetail()
        {
            await NavigationService.NavigateAsync(nameof(ServiceEndpointSettingsPage));
        }
    }
}
