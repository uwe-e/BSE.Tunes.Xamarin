using System;
using System.Collections.Generic;
using System.Text;
using BSE.Tunes.XApp.Services;
using Prism.Navigation;

namespace BSE.Tunes.XApp.ViewModels
{
    public class FeaturedItemsUserControlViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;
        private readonly IDataService dataService;

        public FeaturedItemsUserControlViewModel(INavigationService navigationService,
            ISettingsService settingsService,
            IDataService dataService) : base(navigationService)
        {
            this.settingsService = settingsService;
            this.dataService = dataService;

            LoadData();
        }

        private async void LoadData()
        {
            var tests = await this.dataService.GetFeaturedAlbums(6);
        }

    }
}
