using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSE.Tunes.XApp.ViewModels
{
	public class HomePageViewModel : ViewModelBase
	{
        public HomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
