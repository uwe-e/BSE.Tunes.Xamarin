using BSE.Tunes.XApp.Services;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class MenuViewModel : ViewModelBase
	{
        public MenuViewModel(INavigationService navigationService,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
        }
    }
}
