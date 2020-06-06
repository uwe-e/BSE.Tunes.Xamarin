using BSE.Tunes.XApp.Services;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class BaseSettingsPageViewModel : ViewModelBase, IActiveAware
    {
        private readonly ISettingsService _settingsService;
        private readonly IPlayerManager _playerManager;
        private bool _isActive;
        private bool _isActivated;
        private DelegateCommand _deleteCommand;

        public ISettingsService SettingsService
        {
            get
            {
                return _settingsService;
            }
        }

        public IPlayerManager PlayerManager
        {
            get
            {
                return _playerManager;
            }
        }
        
        public ICommand DeleteCommand
            => _deleteCommand ?? (_deleteCommand = new DelegateCommand(Delete));

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }

        public event EventHandler IsActiveChanged;
        
        public BaseSettingsPageViewModel(
            INavigationService navigationService,
            ISettingsService settingsService,
            IPlayerManager playerManager,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
            _settingsService = settingsService;
            _playerManager = playerManager;
        }
        
        public virtual void DeleteSettings()
        {
        }

        public virtual void LoadSettings()
        {
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
        
        private void Delete()
        {
            DeleteSettings();
        }
    }
}
