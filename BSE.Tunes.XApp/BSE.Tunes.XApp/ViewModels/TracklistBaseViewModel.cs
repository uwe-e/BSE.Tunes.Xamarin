using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.ViewModels
{
    public class TracklistBaseViewModel : ViewModelBase
    {
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private readonly IPlayerManager _playerManager;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<GridPanel> _items;
        private string _image;
        private DelegateCommand<object> _openFlyoutCommand;
        private DelegateCommand _playAllCommand;
        private DelegateCommand _playAllRandomizedCommand;
        private DelegateCommand<GridPanel> _playCommand;

        public DelegateCommand<GridPanel> PlayCommand => _playCommand
            ?? (_playCommand = new DelegateCommand<GridPanel>(PlayTrack));

        public DelegateCommand PlayAllCommand => _playAllCommand
            ?? (_playAllCommand = new DelegateCommand(PlayAll, CanPlayAll));

        public DelegateCommand PlayAllRandomizedCommand => _playAllRandomizedCommand
            ?? (_playAllRandomizedCommand = new DelegateCommand(PlayAllRandomized, CanPlayAllRandomized));

        public DelegateCommand<object> OpenFlyoutCommand => _openFlyoutCommand
            ?? (_openFlyoutCommand = new DelegateCommand<object>(OpenFlyout));

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                SetProperty<string>(ref _image, value);
            }
        }

        public TracklistBaseViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IFlyoutNavigationService flyoutNavigationService,
            IPlayerManager playerManager,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _flyoutNavigationService = flyoutNavigationService;
            _playerManager = playerManager;
            _eventAggregator = eventAggregator;
        }
        
        protected virtual void PlayTrack(GridPanel obj)
        {
        }

        protected virtual void PlayAll()
        {
        }

        protected virtual bool CanPlayAll()
        {
            return Items.Count > 0;
        }

        protected virtual bool CanPlayAllRandomized()
        {
            return CanPlayAll();
        }

        protected virtual void PlayAllRandomized()
        {
        }

        protected virtual void PlayTracks(IEnumerable<int> trackIds, AudioPlayerMode audioPlayerMode)
        {
            _playerManager.PlayTracks(
                            new ObservableCollection<int>(trackIds),
                            audioPlayerMode);
        }

        protected virtual ObservableCollection<int> GetTrackIds()
        {
            throw new System.NotImplementedException();
        }

        protected async virtual void OpenFlyout(object obj)
        {
            var source = obj;
            if (obj is GridPanel item)
            {
                source = item.Data;
            }

            var navigationParams = new NavigationParameters
                    {
                        { "source", source }
                    };

            await _flyoutNavigationService.ShowFlyoutAsync(nameof(ManageAlbumsPage), navigationParams);
        }
    }
}
