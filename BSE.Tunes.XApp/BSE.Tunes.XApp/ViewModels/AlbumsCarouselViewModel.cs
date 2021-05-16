using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Events;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumsCarouselViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;
        private ObservableCollection<GridPanel> _items;
        private ICommand _selectItemCommand;

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public ICommand SelectItemCommand => _selectItemCommand
            ?? (_selectItemCommand = new Xamarin.Forms.Command<GridPanel>(SelectItem));

        public AlbumsCarouselViewModel(INavigationService navigationService,
            IEventAggregator eventAggregator,
            IResourceService resourceService,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _dataService = dataService;

            LoadData();
        }

        private async void LoadData()
        {
            var albums = await this._dataService.GetFeaturedAlbums(6);
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
                            ImageSource = this._dataService.GetImage(album.AlbumId)?.AbsoluteUri,
                            Data = album
                        });
                    }
                }
                IsBusy = false;
            }
        }
        private void SelectItem(GridPanel obj)
        {
            if (obj?.Data is Album album)
            {
                _eventAggregator.GetEvent<AlbumSelectedEvent>().Publish(album);
            }
        }
    }
}
