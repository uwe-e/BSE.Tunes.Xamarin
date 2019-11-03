using BSE.Tunes.XApp.Services;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService,
            IResourceService resourceService)
            : base(navigationService, resourceService)
        {
            Title = "Main Page";
        }
    }
}
