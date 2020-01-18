using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class AlbumDetailPageViewModel : ViewModelBase
    {
        public AlbumDetailPageViewModel(INavigationService navigationService, IResourceService resourceService) : base(navigationService, resourceService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            //base.OnNavigatedTo(parameters);
        }

        //public override void OnNavigatedFrom(INavigationParameters parameters)
        //{
        //    base.OnNavigatedFrom(parameters);
        //}
    }
}
