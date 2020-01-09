using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class FeaturedAlbumsViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;
        private readonly IDataService dataService;
        private ObservableCollection<GridPanel> _items;
        private ICommand _selectItemCommand;

        public ObservableCollection<GridPanel> Items => _items
            ?? (_items = new ObservableCollection<GridPanel>());

        public ICommand SelectItemCommand => _selectItemCommand
            ?? (_selectItemCommand = new Xamarin.Forms.Command<GridPanel>(SelectItem));

        public FeaturedAlbumsViewModel(INavigationService navigationService,
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
            var albums = await this.dataService.GetNewestAlbums(20);
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
                            ImageSource = this.dataService.GetImage(album.AlbumId, true)?.AbsoluteUri
                        });
                    }
                }
            }
        }

        private void SelectItem(GridPanel obj)
        {
            //throw new NotImplementedException();
        }
    }
}
