using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ManageAlbumsPageViewModel : ViewModelBase
    {
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private readonly IDataService _dataService;
        private ICommand _closeFlyoutCommand;
        private ICommand _addToPlaylistCommand;
        private Track _track;
        private string _imageSource;

        public ICommand CloseFlyoutCommand => _closeFlyoutCommand ?? (_closeFlyoutCommand = new DelegateCommand(CloseFlyout));

        public ICommand AddToPlaylistCommand => _addToPlaylistCommand ?? (_addToPlaylistCommand = new DelegateCommand(AddToPlaylist));

        public Track Track
        {
            get
            {
                return _track;
            }
            set
            {
                SetProperty<Track>(ref _track, value);
            }
        }

        public string ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                SetProperty<string>(ref _imageSource, value);
            }
        }

        public ManageAlbumsPageViewModel(
            INavigationService navigationService,
            IFlyoutNavigationService flyoutNavigationService,
            IDataService dataService,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
            _flyoutNavigationService = flyoutNavigationService;
            _dataService = dataService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Track = parameters.GetValue<Track>("track");
            ImageSource = _dataService.GetImage(Track.Album.AlbumId, true)?.AbsoluteUri;
            base.OnNavigatedTo(parameters);
        }

        private async void CloseFlyout()
        {
            await _flyoutNavigationService.CloseFlyoutAsync();

            //await NavigationService.GoBackAsync(useModalNavigation: true, animated: false);
        }
        
        private void AddToPlaylist()
        {
        }
    }
}
