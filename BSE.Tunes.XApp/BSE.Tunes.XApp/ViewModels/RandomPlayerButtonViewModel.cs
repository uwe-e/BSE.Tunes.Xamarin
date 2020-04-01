using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace BSE.Tunes.XApp.ViewModels
{
    public class RandomPlayerButtonViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataService _dataService;
        private string _text;
        private ICommand _playRandomCommand;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty<string>(ref _text, value);
            }
        }

        public ICommand PlayRandomCommand => _playRandomCommand ?? (_playRandomCommand = new DelegateCommand(PlayRandom));

        public RandomPlayerButtonViewModel(INavigationService navigationService,
            IEventAggregator eventAggregator,
            IResourceService resourceService,
            IDataService dataService) : base(navigationService, resourceService)
        {
            this._eventAggregator = eventAggregator;
            this._dataService = dataService;

            LoadData();
        }

        private async void LoadData()
        {
            var sysInfo = await _dataService.GetSystemInfo();
            if (sysInfo != null)
            {
                Text = string.Format(ResourceService.GetString("HomePage_RandomPlayerButton_Button_Text"), sysInfo.NumberTracks);
            }
        }
        
        private void PlayRandom()
        {
        }
    }
}
