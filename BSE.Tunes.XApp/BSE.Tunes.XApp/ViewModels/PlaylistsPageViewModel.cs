using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Extensions;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistsPageViewModel : ViewModelBase, IActiveAware
    {
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IImageService _imageService;
        private ObservableCollection<GridPanel> _items;
        private bool _isActive;
        private bool _isActivated;
        private int _pageSize;
        private int _pageNumber;
        private bool _hasItems;
        private ICommand _loadMoreItemsCommand;
        private ICommand _selectItemCommand;

        public event EventHandler IsActiveChanged;

        public ICommand LoadMoreItemsCommand => _loadMoreItemsCommand ?? (
            _loadMoreItemsCommand = new DelegateCommand(() =>
            {
                Device.BeginInvokeOnMainThread(() => LoadMoreItems());
            }));

        public ICommand SelectItemCommand => _selectItemCommand
            ?? (_selectItemCommand = new Xamarin.Forms.Command<GridPanel>(SelectItem));

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, RaiseIsActiveChanged);
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public PlaylistsPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            ISettingsService settingsService,
            IEventAggregator eventAggregator,
            IImageService imageService) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
            _imageService = imageService;
            _pageSize = 10;

            _eventAggregator.GetEvent<PlaylistActionContextChanged>().Subscribe(args =>
            {
                Items.Clear();
                IsActive = _isActivated = false;
                RaiseIsActiveChanged();
            });

            _eventAggregator.GetEvent<CacheChangedEvent>().Subscribe((args) =>
            {
                Items.Clear();
                IsActive = _isActivated = false;
                RaiseIsActiveChanged();
            });

            _eventAggregator.GetEvent<AlbumInfoSelectionEvent>().ShowAlbum(async (uniqueTrack) =>
            {
                if (PageUtilities.IsCurrentPageTypeOf(typeof(PlaylistsPage)))
                {
                    var navigationParams = new NavigationParameters
                    {
                        { "album", uniqueTrack.Album }
                    };

                    await NavigationService.NavigateAsync(nameof(AlbumDetailPage), navigationParams);
                }
            });
        }

        protected virtual void RaiseIsActiveChanged()
        {
            if (IsActive && !_isActivated)
            {
                _isActivated = true;

                IsBusy = false;
                _hasItems = true;
                _pageNumber = 0;

                LoadMoreItems();
            }
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void LoadMoreItems()
        {
            if (IsBusy)
            {
                return;
            }

            if (_hasItems)
            {
                IsBusy = true;

                try
                {
                    var playlists = await _dataService.GetPlaylistsByUserName(_settingsService.User.UserName, _pageNumber, PageSize);
                    if (playlists == null || playlists.Count == 0)
                    {
                        _hasItems = false;
                        return;
                    }

                    foreach (var playlst in playlists)
                    {
                        if (playlst != null)
                        {
                            var playlist = await _dataService.GetPlaylistByIdWithNumberOfEntries(playlst.Id, _settingsService.User.UserName);
                            if (playlist != null)
                            {
                                Items.Add(new GridPanel
                                {
                                    Title = playlist.Name,
                                    SubTitle = FormatNumberOfEntriesString(playlist),
                                    ImageSource = await _imageService.GetStitchedBitmapSource(playlist.Id),
                                    Data = playlist
                                });
                            }
                        }
                    }
                    _pageNumber = Items.Count;
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
        
        private async void SelectItem(GridPanel obj)
        {
            if (obj?.Data is Playlist playlist)
            {
                var navigationParams = new NavigationParameters
                    {
                        { "playlist", playlist }
                    };

                await NavigationService.NavigateAsync($"{nameof(PlaylistDetailPage)}", navigationParams);
            }
        }

        public virtual string FormatNumberOfEntriesString(Playlist playlist)
        {
            int numberOfEntries = 0;
            if (playlist != null)
            {
                numberOfEntries = playlist.NumberEntries;
            }
            return $"{numberOfEntries} {ResourceService.GetString("PlaylistItem_PartNumberOfEntries")}";
        }
    }
}
