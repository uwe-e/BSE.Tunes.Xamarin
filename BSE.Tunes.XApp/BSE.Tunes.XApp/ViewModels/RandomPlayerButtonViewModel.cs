using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class RandomPlayerButtonViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerManager _playerManager;
        private readonly IDataService _dataService;
        private string _text;
        private ICommand _playRandomCommand;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty<string>(ref _text, value);
            }
        }

        public ICommand PlayRandomCommand => _playRandomCommand ?? (_playRandomCommand = new DelegateCommand(PlayRandom));

        public RandomPlayerButtonViewModel(INavigationService navigationService,
            IEventAggregator eventAggregator,
            IResourceService resourceService,
            IPlayerManager playerManager,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _playerManager = playerManager;
            _dataService = dataService;

            LoadData();
        }

        private async void LoadData()
        {
            ObservableCollection<int> trackIds = await _dataService.GetTrackIdsByGenre();
            if (trackIds != null)
            {
                var randomTrackIds = trackIds.ToRandomCollection();
                int trackId = randomTrackIds.FirstOrDefault();
                if (trackId > 0)
                {
                    //_currentTrack = await _dataService.GetTrackById(trackId);
                }
                //_playlist = randomTrackIds.ToNavigableCollection();
                _playerManager.Playlist = randomTrackIds.ToNavigableCollection();
            }

            var sysInfo = await _dataService.GetSystemInfo();
            if (sysInfo != null)
            {
                Text = string.Format(ResourceService.GetString("HomePage_RandomPlayerButton_Button_Text"), sysInfo.NumberTracks);
            }
        }
        
        private void PlayRandom()
        {
        }
    }
}
