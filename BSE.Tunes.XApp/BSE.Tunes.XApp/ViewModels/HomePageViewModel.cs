using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase
	{
        private readonly IEventAggregator _eventAggregator;
        private ICommand _refreshCommand;
        private bool _isRefreshing;

        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(RefreshView));

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                SetProperty<bool>(ref _isRefreshing, value);
            }
        }

        public HomePageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AlbumSelectedEvent>().Subscribe(SelectAlbum, ThreadOption.UIThread);
            _eventAggregator.GetEvent<PlaylistSelectedEvent>().Subscribe(SelectPlaylist, ThreadOption.UIThread);

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (uniqueTrack) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(HomePage)))
                {
                    var navigationParams = new NavigationParameters         
                    {
                        { "album", uniqueTrack.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
        }

        private async void SelectAlbum(Album album)
        {
            var navigationParams = new NavigationParameters
            {
                { "album", album }
            };
            await NavigationService.NavigateAsync($"{nameof(AlbumDetailPage)}", navigationParams);
        }

        private async void SelectPlaylist(Playlist playlist)
        {
            var navigationParams = new NavigationParameters
                    {
                        { "playlist", playlist }
                    };
            await NavigationService.NavigateAsync($"{nameof(PlaylistDetailPage)}", navigationParams);
        }
        
        private void RefreshView()
        {
            _eventAggregator.GetEvent<HomePageRefreshEvent>().Publish();
            IsRefreshing = false;
        }
    }
}
