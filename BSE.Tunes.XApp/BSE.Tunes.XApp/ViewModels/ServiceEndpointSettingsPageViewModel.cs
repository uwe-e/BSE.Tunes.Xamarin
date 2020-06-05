using BSE.Tunes.XApp.Services;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ServiceEndpointSettingsPageViewModel : ViewModelBase, IActiveAware
    {
        private readonly ISettingsService _settingsService;
        private readonly IPageDialogService _pageDialogService;
        private readonly IPlayerManager _playerManager;
        private bool _isActive;
        private bool _isActivated;
        private string _serviceEndPoint;
        private DelegateCommand _deleteCommand;

        public ICommand DeleteAddressCommand
            => _deleteCommand ?? (_deleteCommand = new DelegateCommand(Delete));

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

        public event EventHandler IsActiveChanged;

        public ServiceEndpointSettingsPageViewModel(
            INavigationService navigationService,
            ISettingsService settingsService,
            IPageDialogService pageDialogService,
            IPlayerManager playerManager,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
            _settingsService = settingsService;
            _pageDialogService = pageDialogService;
            _playerManager = playerManager;
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
        }
        
        private async void Delete()
        {
            ResourceService.GetString("ServiceEndpointSettingsPage_ActionSheet_Title");

            var buttons = new IActionSheetButton[]
            {
                ActionSheetButton.CreateCancelButton(
                    ResourceService.GetString("ActionSheetButton_Cancel")),
                ActionSheetButton.CreateDestroyButton(
                    ResourceService.GetString("ActionSheetButton_Delete"),
                    DeleteAction)
            };

            await _pageDialogService.DisplayActionSheetAsync(
                ResourceService.GetString("ServiceEndpointSettingsPage_ActionSheet_Title"),
                buttons);
        }

        private void DeleteAction()
        {
            //_playerManager.
        }

    }
}
