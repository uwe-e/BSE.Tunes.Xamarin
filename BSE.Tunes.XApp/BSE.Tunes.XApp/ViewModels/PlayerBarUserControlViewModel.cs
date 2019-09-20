using Prism.Commands;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlayerBarUserControlViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        private DelegateCommand _playCommand;
        //IPlaybackController PlaybackController => CrossMediaManager.Current..PlaybackController;


        public DelegateCommand PlayCommand => _playCommand ?? (_playCommand = new DelegateCommand(Play, CanPlay));

        private bool CanPlay()
        {
            //throw new NotImplementedException();
            return true;
        }

        //private string AudioUrl => "http://bse.tunes.webapi//api/files/audio/17f5ac4e-8b8b-42d3-aa15-58223d2ecc4b";
        //
        private string AudioUrl => "http://bse.tunes.webapi/api/files/audio/fd07ea66-e01e-486c-aa69-6c4124224c03";


        private async void Play()
        {
            //throw new NotImplementedException();

            //await CrossMediaManager.Current.Play(AudioUrl);


        }

        public PlayerBarUserControlViewModel(INavigationService navigationService) : base(navigationService)
        {
            this.navigationService = navigationService;
        }
    }
}
