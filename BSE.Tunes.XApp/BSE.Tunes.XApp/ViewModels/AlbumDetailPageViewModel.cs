using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Events;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumDetailPageViewModel : TracklistBaseViewModel
    {
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;
        private Album _album;

        public Album Album
        {
            get
            {
                return _album;
            }
            set
            {
                SetProperty<Album>(ref _album, value);
            }
        }

        public AlbumDetailPageViewModel(INavigationService navigationService,
            IFlyoutNavigationService flyoutNavigationService,
            IEventAggregator eventAggregator,
            IResourceService resourceService,
            IDataService dataService,
            IPlayerManager playerManager) : base(
                navigationService,
                resourceService,
                flyoutNavigationService,
                playerManager,
                eventAggregator)
        {
            _flyoutNavigationService = flyoutNavigationService;
            _eventAggregator = eventAggregator;
            _dataService = dataService;

            _eventAggregator.GetEvent<AddTrackToPlaylistEvent>().Subscribe(SelectPlaylist);
            _eventAggregator.GetEvent<AddAlbumToPlaylistEvent>().Subscribe(SelectPlaylist);
            //_eventAggregator.GetEvent<SelectedToPlaylistEvent>().Subscribe(AddToPlaylist);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            Album album = parameters.GetValue<Album>("album");
            if (album != null)
            {
                Album = await _dataService.GetAlbumById(album.Id);
                foreach(Track track in Album.Tracks)
                {
                    track.Album = new Album
                    {
                        AlbumId = Album.AlbumId,
                        Id = Album.Id,
                        Title = Album.Title,
                        Artist = Album.Artist
                    };
                    Items.Add(new GridPanel
                    {
                        Number = track.TrackNumber,
                        Title = track.Name,
                        Data = track

                    });
                }
                Image = _dataService.GetImage(Album.AlbumId)?.AbsoluteUri;
                PlayAllCommand.RaiseCanExecuteChanged();
                PlayAllRandomizedCommand.RaiseCanExecuteChanged();
                IsBusy = false;
            }
        }

        protected override void PlayTrack(GridPanel obj)
        {
            if (obj?.Data is Track track)
            {
                PlayTracks(new List<int>
                {
                    track.Id
                }, AudioPlayerMode.Song);
            }
        }
        
        protected override void PlayAll()
        {
            PlayTracks(GetTrackIds(), AudioPlayerMode.CD);
        }

        protected override void PlayAllRandomized()
        {
            PlayTracks(GetTrackIds().ToRandomCollection(), AudioPlayerMode.CD);
        }
        
        protected override  ObservableCollection<int> GetTrackIds()
        {
            return new ObservableCollection<int>(Items.Select(track => ((Track)track.Data).Id));
        }

        private async void SelectPlaylist(object obj)
        {
            await _flyoutNavigationService.CloseFlyoutAsync();
            var navigationParams = new NavigationParameters
            {
                { "source", obj }
            };
            await NavigationService.NavigateAsync(nameof(PlaylistSelectorDialogPage), navigationParams, useModalNavigation: true);
        }

    }
}
