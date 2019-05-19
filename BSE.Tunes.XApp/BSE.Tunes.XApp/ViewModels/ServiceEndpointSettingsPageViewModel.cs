using System;
using BSE.Tunes.XApp.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace BSE.Tunes.XApp.ViewModels
{
    public class ServiceEndpointSettingsPageViewModel : ViewModelBase
    {
        private string serviceEndPoint;
        private DelegateCommand saveCommand;
        private readonly IPageDialogService pageDialogService;
        private readonly ISettingsService settingsService;
        private readonly IDataService dataService;

        public string ServiceEndPoint
        {
            get
            {
                return this.serviceEndPoint;
            }
            set
            {
                SetProperty(ref this.serviceEndPoint, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand => this.saveCommand ?? (this.saveCommand = new DelegateCommand(Save, CanSave));

        private bool CanSave()
        {
            return !String.IsNullOrEmpty(ServiceEndPoint);
        }

        public ServiceEndpointSettingsPageViewModel(INavigationService navigationService,
            IPageDialogService pageDialogService,
            ISettingsService settingsService,
            IDataService dataService) : base(navigationService)
        {
            this.pageDialogService = pageDialogService;
            this.settingsService = settingsService;
            this.dataService = dataService;
        }

        private async void Save()
        {
            var uriBuilder = new UriBuilder(ServiceEndPoint);
            var serviceEndPoint = uriBuilder.Uri.AbsoluteUri;
            try
            {
                await this.dataService.IsEndPointAccessibleAsync(serviceEndPoint);
                this.settingsService.ServiceEndPoint = serviceEndPoint;
            }
            catch (Exception)
            {
                await this.pageDialogService.DisplayAlertAsync("test", "message", "cancel");
            }
        }
    }
}
