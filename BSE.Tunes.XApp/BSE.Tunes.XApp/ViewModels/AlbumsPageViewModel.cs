using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumsPageViewModel : ViewModelBase, IActiveAware
    {
        private ObservableCollection<GridPanel> _items;
        private bool _isActive;
        private bool _isActivated;
        private ICommand _loadMoreItemsCommand;
        private ICommand _selectItemCommand;
        private int _totalNumberOfItems;
        private int _pageSize;
        private int _pageNumber;
        private bool _hasItems;
        private readonly IImageService _imageService;
        private readonly IDataService _dataService;

        public event EventHandler IsActiveChanged;
        
        public ICommand LoadMoreItemsCommand => 
            _loadMoreItemsCommand ?? (_loadMoreItemsCommand = new DelegateCommand(LoadMoreItems, HasMoreItems));
        
        public ICommand SelectItemCommand => _selectItemCommand
            ?? (_selectItemCommand = new Xamarin.Forms.Command<GridPanel>(SelectItem));

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }

        public bool HasItems
        {
            get { return _hasItems; }
            set { SetProperty(ref _hasItems, value); }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { SetProperty(ref _pageSize, value); }
        }

        public int PageNumber
        {
            get { return _pageNumber; }
            set { SetProperty(ref _pageNumber, value); }
        }

        public int TotalNumberOfItems
        {
            get { return _totalNumberOfItems; }
            set { SetProperty(ref _totalNumberOfItems, value); }
        }

        public AlbumsPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IImageService imageService,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _imageService = imageService;
            _dataService = dataService;
            PageSize = 10;
        }

        protected async virtual void RaiseIsActiveChanged()
        {
            if (IsActive && !_isActivated)
            {
                _isActivated = true;

                await LoadAlbums(null);
            }
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task LoadAlbums(int? genreId)
        {
            TotalNumberOfItems = await _dataService.GetNumberOfAlbumsByGenre(genreId);
            HasItems = TotalNumberOfItems > 0;
            if (HasItems)
            {
                IsBusy = false;
                LoadMoreItems();
            }
        }
        
        private bool HasMoreItems()
        {
            return HasItems && TotalNumberOfItems > PageNumber;
        }

        private async void LoadMoreItems()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            try
            {
                var albums = await _dataService.GetAlbumsByGenre(null, PageNumber, PageSize);
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
                                ImageSource = _imageService.GetBitmapSource(album.AlbumId),
                                Data = album
                            });
                        }
                    }
                    PageNumber = Items.Count;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        private async void SelectItem(GridPanel obj)
        {
            if (obj?.Data is Album album)
            {
                var navigationParams = new NavigationParameters
                    {
                        { "album", album }
                    };
                //await NavigationService.NavigateAsync("NavigationPage/AlbumDetailPage", navigationParams);
                await NavigationService.NavigateAsync($"{nameof(AlbumDetailPage)}", navigationParams);
            }
        }
    }
}
