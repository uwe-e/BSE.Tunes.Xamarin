using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Events;
using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumSearchResultsPageViewModel : BaseSearchResultsPageViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public AlbumSearchResultsPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IEventAggregator eventAggregator,
            IDataService dataService) : base(navigationService, resourceService, dataService)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (uniqueTrack) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(AlbumSearchResultsPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", uniqueTrack.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
        }

        protected async override Task GetSearchResults()
        {
            var albums = await DataService.GetAlbumSearchResults(Query, PageNumber, PageSize);
            if (albums.Length == 0)
            {
                HasItems = false;
            }

            foreach (var album in albums)
            {
                if (album != null)
                {
                    Items.Add(new GridPanel
                    {
                        Title = album.Title,
                        SubTitle = album.Artist.Name,
                        ImageSource = DataService.GetImage(album.AlbumId, true)?.AbsoluteUri,
                        Data = album
                    });
                }
            }
            PageNumber = Items.Count;
        }

        protected async override void SelectItem(GridPanel obj)
        {
            if (obj?.Data is Album album)
            {
                var navigationParams = new NavigationParameters
                    {
                        { "album", album }
                    };
                await NavigationService.NavigateAsync($"{nameof(AlbumDetailPage)}", navigationParams);
            }
        }
    }
}
