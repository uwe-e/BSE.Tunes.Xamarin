using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPlayerService _playerService;
        private readonly IDataService _dataService;
        private Track _currentTrack;
        private ICommand _playCommand;
        private ICommand _pauseCommand;
        private ICommand _playNextCommand;
        private NavigableCollection<int> _playlist;

        public ICommand PlayCommand => _playCommand ?? (_playCommand = new DelegateCommand(Play, CanPlay));
        public ICommand PauseCommand => _pauseCommand ?? (_pauseCommand = new DelegateCommand(Pause, CanPause));
        public ICommand PlayNextCommand => _playNextCommand ?? (_playNextCommand = new DelegateCommand(PlayNext, CanPlayNext));

        

        public MainPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IPlayerService playerService,
            IDataService dataService)
            : base(navigationService, resourceService)
        {
            _playerService = playerService;
            _dataService = dataService;

            LoadPlaylist();
        }
        
        private bool CanPlay()
        {
            return true;
        }
        
        private void Play()
        {
            _playerService?.SetTrackAsync(_currentTrack);
        }
        
        private bool CanPause()
        {
            return true;
        }

        private void Pause()
        {
        }
        
        private bool CanPlayNext()
        {
            return true;
        }

        private void PlayNext()
        {
        }

        private async void LoadPlaylist()
        {
            //Get the id's of all playable tracks and randomize it
            ObservableCollection<int> trackIds = await _dataService.GetTrackIdsByGenre();
            if (trackIds != null)
            {
                var randomTrackIds = trackIds.ToRandomCollection();
                int trackId = randomTrackIds.FirstOrDefault();
                if (trackId > 0)
                {
                    _currentTrack = await _dataService.GetTrackById(trackId);
                }
                _playlist = randomTrackIds.ToNavigableCollection();
            }
        }
    }
}
