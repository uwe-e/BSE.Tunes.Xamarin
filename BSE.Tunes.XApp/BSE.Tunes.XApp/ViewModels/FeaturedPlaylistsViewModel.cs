using BSE.Tunes.XApp.Services;
using Prism.Events;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class FeaturedPlaylistsViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;

        public FeaturedPlaylistsViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IEventAggregator eventAggregator,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _dataService = dataService;

            LoadData();
        }

        private async void LoadData()
        {
            IsBusy = false;
        }
    }
}
