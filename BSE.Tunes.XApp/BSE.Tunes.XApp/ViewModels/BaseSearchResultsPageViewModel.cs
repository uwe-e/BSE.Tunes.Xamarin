using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class BaseSearchResultsPageViewModel : ViewModelBase
    {
        private ObservableCollection<GridPanel> _items;
        private ICommand _selectItemCommand;
        private ICommand _loadMoreItemsCommand;
        private int _pageSize;
        private int _pageNumber;
        private string _query;
        private bool _hasItems;
        private readonly IDataService _dataService;

        public ObservableCollection<GridPanel> Items => _items ?? (_items = new ObservableCollection<GridPanel>());

        public ICommand LoadMoreItemsCommand =>
            _loadMoreItemsCommand ?? (_loadMoreItemsCommand = new DelegateCommand(LoadMoreItems));

        public ICommand SelectItemCommand => _selectItemCommand
            ?? (_selectItemCommand = new Xamarin.Forms.Command<GridPanel>(SelectItem));

        public int PageSize
        {
            get { return _pageSize; }
            set { SetProperty(ref _pageSize, value); }
        }

        public int PageNumber
        {
            get { return _pageNumber; }
            set { SetProperty(ref _pageNumber, value); }
        }

        protected bool HasItems
        {
            get => _hasItems;
            set => _hasItems = value;
        }

        protected string Query => _query;

        protected IDataService DataService => _dataService;

        public BaseSearchResultsPageViewModel(INavigationService navigationService,
                    IResourceService resourceService,
                    IDataService dataService) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            PageSize = 10;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var query = parameters.GetValue<string>("query");
            if (!string.IsNullOrEmpty(query))
            { 
                if (query.CompareTo(_query) != 0)
                {
                    IsBusy = false;
                    _query = query;
                    _hasItems = true;
                    PageNumber = 0;
                    Items.Clear();
                    LoadMoreItems();
                }
            }
        }

        protected virtual Task GetSearchResults()
        {
            return Task.CompletedTask;
        }

        protected virtual void SelectItem(GridPanel obj)
        {
        }

        private async void LoadMoreItems()
        {
            if (IsBusy)
            {
                return;
            }

            if (_hasItems == true)
            {

                IsBusy = true;

                try
                {
                    await GetSearchResults();
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
