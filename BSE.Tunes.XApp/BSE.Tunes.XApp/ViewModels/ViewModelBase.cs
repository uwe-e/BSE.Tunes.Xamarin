using BSE.Tunes.XApp.Services;
using Prism.Mvvm;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        private string _title;
        private bool _isBusy;
        private readonly IResourceService _resourceService;
        private readonly INavigationService _navigationService;

        protected INavigationService NavigationService
        {
            get
            {
                return _navigationService;
            }
        }

        public IResourceService ResourceService
        {
            get
            {
                return _resourceService;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                SetProperty<bool>(ref _isBusy, value);
            }
        }

        public ViewModelBase(INavigationService navigationService, IResourceService resourceService)
        {
            _navigationService = navigationService;
            _resourceService = resourceService;
            _isBusy = true;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }
        //https://github.com/PrismLibrary/Prism/releases/tag/v7.2.0.1367
        //Removed with Prism 7.2
        //public virtual void OnNavigatingTo(INavigationParameters parameters)
        //{

        //}

        public virtual void Destroy()
        {

        }
    }
}
