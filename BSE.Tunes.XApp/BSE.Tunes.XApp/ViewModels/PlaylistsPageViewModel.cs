using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistsPageViewModel : ViewModelBase, IActiveAware
    {
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private ObservableCollection<GridPanel> _items;
        private bool _isActive;
        private bool _isActivated;
        private int _pageSize;
        private int _pageNumber;
        private bool _hasItems;
        private ICommand _loadMoreItemsCommand;

        public event EventHandler IsActiveChanged;

        public ICommand LoadMoreItemsCommand =>
            _loadMoreItemsCommand ?? (_loadMoreItemsCommand = new DelegateCommand(LoadMoreItems));

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { SetProperty(ref _pageSize, value); }
        }

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public PlaylistsPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            ISettingsService settingsService) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _pageSize = 10;
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

                    foreach (var playlist in playlists)
                    {
                        if (playlist != null)
                        {
                            Items.Add(new GridPanel
                            {
                                Title = playlist.Name,
                                SubTitle = FormatNumberOfEntriesString(playlist),
                                //ImageSource = DataService.GetImage(track.Album.AlbumId)?.AbsoluteUri,
                                Data = playlist
                            });
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
