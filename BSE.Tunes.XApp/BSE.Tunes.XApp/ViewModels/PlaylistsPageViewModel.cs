using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class PlaylistsPageViewModel : ViewModelBase
    {
        public PlaylistsPageViewModel(INavigationService navigationService, IResourceService resourceService) : base(navigationService, resourceService)
        {
        }
    }
}
