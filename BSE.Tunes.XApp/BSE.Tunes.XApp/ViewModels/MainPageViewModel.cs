using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private ICommand _playCommand;
        private ICommand _pauseCommand;
        private ICommand _playNextCommand;

        public ICommand PlayCommand => _playCommand ?? (_playCommand = new DelegateCommand(Play, CanPlay));
        public ICommand PauseCommand => _pauseCommand ?? (_pauseCommand = new DelegateCommand(Pause, CanPause));
        public ICommand PlayNextCommand => _playNextCommand ?? (_playNextCommand = new DelegateCommand(PlayNext, CanPlayNext));

        

        public MainPageViewModel(INavigationService navigationService,
            IResourceService resourceService)
            : base(navigationService, resourceService)
        {
            Title = "Main Page";
        }
        
        private bool CanPlay()
        {
            return true;
        }
        
        private void Play()
        {
            
        }
        
        private bool CanPause()
        {
            return true;
        }

        private void Pause()
        {
        }
        
        private bool CanPlayNext()
        {
            return true;
        }

        private void PlayNext()
        {
        }
    }
}
