using Prism;
using Prism.Ioc;
using BSE.Tunes.XApp.ViewModels;
using BSE.Tunes.XApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BSE.Tunes.XApp.Services;

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

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("ExtendedSplashPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ServiceEndpointSettingsPage, ServiceEndpointSettingsPageViewModel>();
            containerRegistry.Register<ISettingsService, SettingsService>();
            containerRegistry.Register<IRequestService, RequestService>();
            containerRegistry.Register<ITunesService, TunesService>();
            containerRegistry.Register<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterForNavigation<ExtendedSplashPage, ExtendedSplashPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
        }
    }
}
