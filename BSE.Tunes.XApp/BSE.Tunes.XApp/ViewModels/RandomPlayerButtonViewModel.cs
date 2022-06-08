using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class RandomPlayerButtonViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerManager _playerManager;
        private readonly IDataService _dataService;
        private string _text;
        private ObservableCollection<int> _trackIds;
        private DelegateCommand _playRandomCommand;

        public DelegateCommand PlayRandomCommand => _playRandomCommand ?? (_playRandomCommand = new DelegateCommand(PlayRandom, CanPlayRandom));

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

        public RandomPlayerButtonViewModel(INavigationService navigationService,
            IEventAggregator eventAggregator,
            IResourceService resourceService,
            IPlayerManager playerManager,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _playerManager = playerManager;
            _dataService = dataService;

            _eventAggregator.GetEvent<HomePageRefreshEvent>().Subscribe(async () =>
            {
                IsBusy = true;
                await LoadSystemInfo();
            });

            LoadData();
        }

        private async void LoadData()
        {
            ObservableCollection<int> trackIds = await _dataService.GetTrackIdsByGenre();
            if (trackIds != null)
            {
                _trackIds = trackIds.ToRandomCollection();
                int trackId = _trackIds.FirstOrDefault();
                if (trackId > 0)
                {
                    var track = await _dataService.GetTrackById(trackId);
                    if (track != null)
                    {
                        _eventAggregator.GetEvent<TrackChangedEvent>().Publish(track);
                    }
                }
                _playerManager.Playlist = _trackIds.ToNavigableCollection();
                PlayRandomCommand.RaiseCanExecuteChanged();
            }

            await LoadSystemInfo();

            IsBusy = false;
        }

        private async Task LoadSystemInfo()
        {
            var sysInfo = await _dataService.GetSystemInfo();
            if (sysInfo != null)
            {
                Text = string.Format(ResourceService.GetString("HomePage_RandomPlayerButton_Button_Text"), sysInfo.NumberTracks);
            }
        }

        private bool CanPlayRandom()
        {
            return _trackIds?.Count > 0;
        }
        
        private void PlayRandom()
        {
            _trackIds = _trackIds?.ToRandomCollection();
            if (_trackIds != null)
            {
                _playerManager.PlayTracks(new ObservableCollection<int>(_trackIds), AudioPlayerMode.Random);
            }
        }
    }
}
