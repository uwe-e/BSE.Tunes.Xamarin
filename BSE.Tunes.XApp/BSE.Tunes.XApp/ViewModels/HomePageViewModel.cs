using BSE.Tunes.XApp.Services;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase
	{
        public HomePageViewModel(INavigationService navigationService, IResourceService resourceService) : base(navigationService, resourceService)
        {
        }
    }
}
