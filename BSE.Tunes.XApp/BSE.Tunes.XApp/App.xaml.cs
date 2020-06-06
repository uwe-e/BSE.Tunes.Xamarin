using Prism;
using Prism.Ioc;
using BSE.Tunes.XApp.ViewModels;
using BSE.Tunes.XApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.Styles;
using Prism.Mvvm;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BSE.Tunes.XApp
{
    public partial class App
    {
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
                "IndicatorView_Experimental"
            });

            await NavigationService.NavigateAsync(nameof(ExtendedSplashPage));
        }

        protected override async void OnStart()
        {
            base.OnStart();

            var theme = await DependencyService.Get<IEnvironment>().GetOperatingSystemThemeAsync();

            SetTheme(theme);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ServiceEndpointWizzardPage, ServiceEndpointSettingsWizzardViewModel>();
            containerRegistry.RegisterForNavigation<ExtendedSplashPage, ExtendedSplashPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<AlbumsPage, AlbumsPageViewModel>();
            containerRegistry.RegisterForNavigation<AlbumDetailPage, AlbumDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<PlaylistsPage, PlaylistsPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<ServiceEndpointSettingsPage, ServiceEndpointSettingsPageViewModel>();

            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
            containerRegistry.Register<IDataService, DataService>();
            containerRegistry.Register<IRequestService, RequestService>();
            containerRegistry.Register<IResourceService, ResourceService>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterSingleton<IPlayerManager, PlayerManager>();

            var playerService = DependencyService.Get<IPlayerService>();
            containerRegistry.RegisterInstance(playerService);

            ViewModelLocationProvider.Register<FeaturedAlbumsView, FeaturedAlbumsViewModel>();
            containerRegistry.RegisterForNavigation<ServiceEndpointSettingsPage, ServiceEndpointSettingsPageViewModel>();
        }

        private void SetTheme(Theme theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    default:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }
            }
        }
    }
}
