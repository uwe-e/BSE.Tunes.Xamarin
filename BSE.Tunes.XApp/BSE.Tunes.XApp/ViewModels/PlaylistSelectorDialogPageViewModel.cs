using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistSelectorDialogPageViewModel : ViewModelBase
    {
        private ICommand _cancelCommand;
        private ICommand _openNewPlaylistDialogCommand;
        private ObservableCollection<FlyoutItemViewModel> _playlistFlyoutItems;
        private PlaylistActionContext _playlistActionContext;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;
        private readonly IPlaylistManager _playlistManager;
        private readonly IEventAggregator _eventAggregator;

        public ICommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(CloseDialog));

        public ICommand OpenNewPlaylistDialogCommand =>
            _openNewPlaylistDialogCommand ?? (_openNewPlaylistDialogCommand = new DelegateCommand(OpenNewPlaylistDialog));

        public virtual ObservableCollection<FlyoutItemViewModel> PlaylistFlyoutItems => 
            _playlistFlyoutItems ?? (_playlistFlyoutItems = new ObservableCollection<FlyoutItemViewModel>());

        public PlaylistSelectorDialogPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService,
            ISettingsService settingsService,
            IImageService imageService,
            IPlaylistManager playlistManager,
            IEventAggregator eventAggregator) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            _settingsService = settingsService;
            _imageService = imageService;
            _playlistManager = playlistManager;
            _eventAggregator = eventAggregator;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            _playlistActionContext = parameters.GetValue<PlaylistActionContext>("source");

            await CreatePlaylistFlyoutItems();

            IsBusy = false;
            
            base.OnNavigatedTo(parameters);
        }

        //protected virtual void AddTracksToPlaylist(Playlist playlist, Track[] tracks)
        //{
        //    if (playlist != null && tracks != null)
        //    {
        //        foreach (var track in tracks)
        //        {
        //            if (track != null)
        //            {
        //                playlist.Entries.Add(new PlaylistEntry
        //                {
        //                    PlaylistId = playlist.Id,
        //                    TrackId = track.Id,
        //                    Guid = Guid.NewGuid()
        //                });
        //            }
        //        }
        //        AppendToPlaylist(playlist);
        //    }
        //}

        //protected virtual async void AppendToPlaylist(Playlist playlist)
        //{
        //    var changedPlaylist = await _dataService.AppendToPlaylist(playlist);


        //    CloseDialog();
        //}

        private async Task CreatePlaylistFlyoutItems()
        {
            var playlists = await _dataService.GetPlaylistsByUserName(_settingsService.User.UserName, 0, 50);
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    if (playlist != null)
                    {
                        var flyoutItem = new FlyoutItemViewModel
                        {
                            Text = playlist.Name,
                            ImageSource = await _imageService.GetStitchedBitmapSource(playlist.Id, 50, true),
                            Data = playlist
                        };
                        flyoutItem.ItemClicked += OnFlyoutItemClicked;
                        PlaylistFlyoutItems.Add(flyoutItem);
                    }
                }
            }
        }

        private void OnFlyoutItemClicked(object sender, EventArgs e)
        {
            if (sender is FlyoutItemViewModel flyoutItem)
            {
                _playlistActionContext.PlaylistTo = flyoutItem.Data as Playlist;
                _playlistActionContext.ActionMode = PlaylistActionMode.AddToPlaylist;
                _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(_playlistActionContext);
            }
        }

        private async void CloseDialog()
        {
            await NavigationService.GoBackAsync(useModalNavigation: true);
        }
        
        private void OpenNewPlaylistDialog()
        {
            _playlistActionContext.ActionMode = PlaylistActionMode.CreatePlaylist;
            _eventAggregator.GetEvent<PlaylistActionContextChanged>().Publish(_playlistActionContext);
        }

    }
}
