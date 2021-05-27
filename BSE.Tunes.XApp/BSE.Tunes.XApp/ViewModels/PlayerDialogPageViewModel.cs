using BSE.Tunes.XApp.Models.Contract;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlayerDialogPageViewModel : PlayerBaseViewModel
    {
        private readonly IImageService _imageService;
        private ICommand _closeDialogCommand;
        private string _coverImage;

        public ICommand CloseDialogCommand => _closeDialogCommand
            ?? (_closeDialogCommand = new DelegateCommand(async() =>
            {
                await NavigationService.GoBackAsync();
            }));
            

        public string CoverImage
        {
            get
            {
                return _coverImage;
            }
            set
            {
                SetProperty<string>(ref _coverImage, value);
            }
        }

        public PlayerDialogPageViewModel(
            INavigationService navigationService,
            IResourceService resourceService,
            IPageDialogService pageDialogService,
            IImageService imageService,
            IEventAggregator eventAggregator,
            IPlayerManager playerManager) : base(navigationService, resourceService, pageDialogService, playerManager, eventAggregator)
        {
            _imageService = imageService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            CurrentTrack = parameters.GetValue<Track>("source");
            if (CurrentTrack != null)
            {
                CoverImage = _imageService.GetBitmapSource(CurrentTrack.Album.AlbumId);
            }
            Progress = PlayerManager.Progress;
            AudioPlayerState = PlayerManager.AudioPlayerState;

            base.OnNavigatedTo(parameters);
        }

        protected override void OnTrackChanged(Track currentTrack)
        {
            if (currentTrack != null)
            {
                CoverImage = _imageService.GetBitmapSource(currentTrack.Album.AlbumId);
            }
        }
    }
}
