using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ManageAlbumsPageViewModel : ViewModelBase
    {
        private readonly IFlyoutNavigationService _flyoutNavigationService;
        private ICommand _closeFlyoutCommand;
        private ICommand _addToPlaylistCommand;

        public ICommand CloseFlyoutCommand => _closeFlyoutCommand ?? (_closeFlyoutCommand = new DelegateCommand(CloseFlyout));

        public ICommand AddToPlaylistCommand => _addToPlaylistCommand ?? (_addToPlaylistCommand = new DelegateCommand(AddToPlaylist));

        public ManageAlbumsPageViewModel(
            INavigationService navigationService,
            IFlyoutNavigationService flyoutNavigationService,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
            _flyoutNavigationService = flyoutNavigationService;
        }
        
        private async void CloseFlyout()
        {
            await _flyoutNavigationService.CloseFlyoutAsync();

            //await NavigationService.GoBackAsync(useModalNavigation: true, animated: false);
        }
        
        private void AddToPlaylist()
        {
        }
    }
}
