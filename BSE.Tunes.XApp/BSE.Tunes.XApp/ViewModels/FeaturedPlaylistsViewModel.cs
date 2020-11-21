using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Events;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class FeaturedPlaylistsViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;
        private readonly IDataService _dataService;
        private ObservableCollection<GridPanel> _items;
        private ICommand _selectItemCommand;

        public ObservableCollection<GridPanel> Items => _items
            ?? (_items = new ObservableCollection<GridPanel>());

        public ICommand SelectItemCommand => _selectItemCommand
            ?? (_selectItemCommand = new Xamarin.Forms.Command<GridPanel>(SelectItem));

        public FeaturedPlaylistsViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IEventAggregator eventAggregator,
            ISettingsService settingsService,
            IImageService imageService,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _imageService = imageService;
            _dataService = dataService;

            LoadData();

            _eventAggregator.GetEvent<PlaylistActionContextChanged>().Subscribe(async args =>
            {
                if (args is PlaylistActionContext managePlaylistContext)
                {
                    if (managePlaylistContext.ActionMode == PlaylistActionMode.PlaylistUpdated)
                    {
                        GridPanel panel = null;
                        if (managePlaylistContext.PlaylistTo is Playlist playlist)
                        {
                            panel = Items.Where(p => p.Id == playlist.Id).FirstOrDefault();
                            if (panel != null)
                            {
                                panel.ImageSource = await _imageService.GetStitchedBitmapSource(playlist.Id);
                            }
                        }
                        if (managePlaylistContext.Data is PlaylistEntry playlistEntry)
                        {
                            panel = Items.Where(p => p.Id == playlistEntry.PlaylistId).FirstOrDefault();
                            if (panel != null)
                            {
                                panel.ImageSource = await _imageService.GetStitchedBitmapSource(playlistEntry.PlaylistId);
                            }
                        }
                        //managePlaylistContext.ActionMode = PlaylistActionMode.None;
                        //if (managePlaylistContext.PlaylistTo == Playlist)
                        {
                            //Items.Select(p => p.Id == )
                        }
                        //if (panel != null)
                        //{
                        //    panel.ImageSource = await _imageService.GetStitchedBitmapSource(
                        //                playlist.Id),
                        //}

                    }
                }
            });
        }

        private async void LoadData()
        {
            Items.Clear();
            var playlists = await _dataService.GetPlaylistsByUserName(_settingsService.User.UserName, 0, 5);
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    Items.Add(new GridPanel
                    {
                        Id = playlist.Id,
                        Title = playlist.Name,
                        SubTitle = $"{playlist.NumberEntries} {ResourceService.GetString("PlaylistItem_PartNumberOfEntries")}",
                        ImageSource = await _imageService.GetStitchedBitmapSource(
                                        playlist.Id),
                        Data = playlist
                    });
                }
            }
            IsBusy = false;
        }
        
        private void SelectItem(GridPanel obj)
        {
            if (obj?.Data is Playlist playlist)
            {
                _eventAggregator.GetEvent<PlaylistSelectedEvent>().Publish(playlist);
            }
        }
    }
}
