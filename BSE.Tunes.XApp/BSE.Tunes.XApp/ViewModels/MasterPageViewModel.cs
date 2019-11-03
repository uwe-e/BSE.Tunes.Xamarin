using BSE.Tunes.XApp.Services;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class MasterPageViewModel : ViewModelBase
	{
        public MasterPageViewModel(INavigationService navigationService,
            IResourceService resourceService) : base(navigationService, resourceService)
        {
        }
    }
}
