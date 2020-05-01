using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Events;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase
	{
        private readonly IEventAggregator _eventAggregator;

        public HomePageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AlbumSelectedEvent>().Subscribe(SelectAlbum, ThreadOption.UIThread);

        }
        private async void SelectAlbum(Album album)
        {
            if (album == null)
                return;

            var navigationParams = new NavigationParameters
            {
                { "album", album }
            };
            await NavigationService.NavigateAsync("NavigationPage/AlbumDetailPage", navigationParams);
        }
        
    }
}
