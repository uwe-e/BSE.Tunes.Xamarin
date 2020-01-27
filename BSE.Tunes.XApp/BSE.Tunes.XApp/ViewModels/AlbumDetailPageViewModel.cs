using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumDetailPageViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private Album _album;
        private ObservableCollection<Track> _tracks;
        private string _coverSource;

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

        public string CoverSource
        {
            get
            {
                return _coverSource;
            }
            set
            {
                SetProperty<string>(ref _coverSource, value);
            }
        }

        public ObservableCollection<Track> Tracks => _tracks ?? (_tracks = new ObservableCollection<Track>());

        public AlbumDetailPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _dataService = dataService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            Album album = parameters.GetValue<Album>("album");
            if (album != null)
            {
                Album = await _dataService.GetAlbumById(album.Id);
                foreach(Track track in Album.Tracks)
                {
                    Tracks.Add(track);
                }
                CoverSource = _dataService.GetImage(Album.AlbumId)?.AbsoluteUri;
                IsBusy = false;
            }
        }
    }
}
