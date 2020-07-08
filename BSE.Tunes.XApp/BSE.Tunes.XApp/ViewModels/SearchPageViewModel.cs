using BSE.Tunes.XApp.Services;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private DelegateCommand<string> _textChangedCommand;
        private string _textValue;
        private readonly IDataService _dataService;
        private String[] _searchSuggestions;

        public DelegateCommand<string> TextChangedCommand => _textChangedCommand
            ?? (_textChangedCommand = new DelegateCommand<string>(TextChanged));

        public string TextValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                SetProperty<string>(ref _textValue, value);
            }
        }

        public String[] SearchSuggestions
        {
            get
            {
                return _searchSuggestions;
            }
            set
            {
                SetProperty<String[]>(ref _searchSuggestions, value);
            }
        }

        public SearchPageViewModel(INavigationService navigationService,
            IResourceService resourceService,
            IDataService dataService) : base(navigationService, resourceService)
        {
            _dataService = dataService;
            IsBusy = false;
        }

        private async void TextChanged(string newTextValue)
        {
            IsBusy = true;
            if (string.IsNullOrEmpty(newTextValue) || newTextValue.Length < 3)
            {
                SearchSuggestions = null;
            }
            else
            {
                SearchSuggestions = await _dataService.GetSearchSuggestions(newTextValue);
            }
            IsBusy = false;
        }
    }
}
