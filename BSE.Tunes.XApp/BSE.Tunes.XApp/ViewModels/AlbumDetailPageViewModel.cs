using BSE.Tunes.XApp.Collections;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumDetailPageViewModel : TracklistBaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IImageService _imageService;
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
            IImageService imageService,
            IPlayerManager playerManager) : base(
                navigationService,
                resourceService,
                flyoutNavigationService,
                dataService,
                playerManager,
                imageService,
                eventAggregator)
        {
            _dataService = dataService;
            _imageService = imageService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            Album album = parameters.GetValue<Album>("album");
            if (album != null)
            {
                Album = await _dataService.GetAlbumById(album.Id);
                foreach (Track track in Album.Tracks)
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
                Image = _imageService.GetBitmapSource(Album.AlbumId);
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

        protected override ObservableCollection<int> GetTrackIds()
        {
            return new ObservableCollection<int>(Items.Select(track => ((Track)track.Data).Id));
        }

        private void CloseNewPlaylistDialogCallback(IDialogResult obj)
        {
            //throw new NotImplementedException();
        }
    }
}
