using Prism;
using Prism.Ioc;
using BSE.Tunes.XApp.ViewModels;
using BSE.Tunes.XApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Styles;
using Prism.Mvvm;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BSE.Tunes.XApp
{
    public partial class App
    {
        private OSAppTheme _currentTheme;
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }
        /*
         * "setFormsDependencyResolver = true" means that the Xamarin.Forms DepedencyService will
         * use the same DI container when resolving instances.
         */
        public App(IPlatformInitializer initializer) : base(initializer, setFormsDependencyResolver: true) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            // Enable your flags here!
            Device.SetFlags(new[] {
                "CarouselView_Experimental",
                "IndicatorView_Experimental",
                "AppTheme_Experimental"
            });

            if (Application.Current != null)
            {
                Application.Current.RequestedThemeChanged += HandleRequestedThemeChanged;
                _currentTheme = Application.Current.RequestedTheme;
                SetTheme(_currentTheme);
            }

            await NavigationService.NavigateAsync(nameof(ExtendedSplashPage));
        }

        private void HandleRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            if (e.RequestedTheme != _currentTheme)
            {
                _currentTheme = e.RequestedTheme;
                SetTheme(_currentTheme);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ServiceEndpointWizzardPage, ServiceEndpointSettingsWizzardViewModel>();
            containerRegistry.RegisterForNavigation<ExtendedSplashPage, ExtendedSplashPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginWizzardPage, LoginWizzardPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<AlbumsPage, AlbumsPageViewModel>();
            containerRegistry.RegisterForNavigation<AlbumDetailPage, AlbumDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<PlaylistsPage, PlaylistsPageViewModel>();
            containerRegistry.RegisterForNavigation<SearchPage, SearchPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<ServiceEndpointSettingsPage, ServiceEndpointSettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<ServiceEndpointSettingsPage, ServiceEndpointSettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginSettingsPage, LoginSettingsPageViewModel>();

            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
            containerRegistry.Register<IDataService, DataService>();
            containerRegistry.Register<IRequestService, RequestService>();
            containerRegistry.Register<IResourceService, ResourceService>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterSingleton<IPlayerManager, PlayerManager>();

            var playerService = DependencyService.Get<IPlayerService>();
            containerRegistry.RegisterInstance(playerService);

            ViewModelLocationProvider.Register<FeaturedAlbumsView, FeaturedAlbumsViewModel>();

            containerRegistry.RegisterForNavigation<AlbumSearchResultsPage, AlbumSearchResultsPageViewModel>();
            containerRegistry.RegisterForNavigation<TrackSearchResultsPage, TrackSearchResultsPageViewModel>();
        }

        private void SetTheme(OSAppTheme theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                switch (theme)
                {
                    case OSAppTheme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    case OSAppTheme.Light:
                    default:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }
            }
        }
    }
}
