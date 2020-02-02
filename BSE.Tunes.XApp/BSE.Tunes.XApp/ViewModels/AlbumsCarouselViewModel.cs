using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumsCarouselViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;
        private readonly IDataService dataService;
        private ObservableCollection<GridPanel> m_items;

        public ObservableCollection<GridPanel> Items => m_items ?? (m_items = new ObservableCollection<GridPanel>());

        public AlbumsCarouselViewModel(INavigationService navigationService,
            IResourceService resourceService,
            ISettingsService settingsService,
            IDataService dataService) : base(navigationService, resourceService)
        {
            this.settingsService = settingsService;
            this.dataService = dataService;

            LoadData();
        }

        private async void LoadData()
        {
            var albums = await this.dataService.GetFeaturedAlbums(6);
            if (albums != null)
            {
                foreach (var album in albums)
                {
                    if (album != null)
                    {
                        Items.Add(new GridPanel
                        {
                            Title = album.Title,
                            SubTitle = album.Artist.Name,
                            ImageSource = this.dataService.GetImage(album.AlbumId)?.AbsoluteUri
                        });
                    }
                }
                IsBusy = false;
            }
        }

    }
}
