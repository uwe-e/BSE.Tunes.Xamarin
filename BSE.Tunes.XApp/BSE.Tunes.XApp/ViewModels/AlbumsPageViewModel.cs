using BSE.Tunes.XApp.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumsPageViewModel : ViewModelBase
    {
        public AlbumsPageViewModel(INavigationService navigationService, IResourceService resourceService) : base(navigationService, resourceService)
        {
        }
    }
}
