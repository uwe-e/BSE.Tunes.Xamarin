using Prism.Mvvm;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService
        {
            get; private set;
        }

        private string _title;
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

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
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
