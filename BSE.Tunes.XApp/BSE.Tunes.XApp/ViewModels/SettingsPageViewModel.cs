using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(INavigationService navigationService, IResourceService resourceService) : base(navigationService, resourceService)
        {
        }
    }
}
