using BSE.Tunes.XApp.iOS.Services;
using BSE.Tunes.XApp.Services;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(Environment_iOS))]
namespace BSE.Tunes.XApp.iOS.Services
{

    public class Environment_iOS : IEnvironment
    {
        public Task<Theme> GetOperatingSystemThemeAsync() => Device.InvokeOnMainThreadAsync(GetOperatingSystemTheme);

        public Theme GetOperatingSystemTheme()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                var currentUIViewController = GetVisibleViewController();

                var userInterfaceStyle = currentUIViewController.TraitCollection.UserInterfaceStyle;

                switch (userInterfaceStyle)
                {
                    case UIUserInterfaceStyle.Light:
                        return Theme.Light;
                    case UIUserInterfaceStyle.Dark:
                        return Theme.Dark;
                    default:
                        throw new NotSupportedException($"UIUserInterfaceStyle {userInterfaceStyle} not supported");
                }
            }
            else
            {
                return Theme.Light;
            }
        }

        private static UIViewController GetVisibleViewController()
        {

            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            switch (rootController.PresentedViewController)
            {
                case UINavigationController navigationController:
                    return navigationController.TopViewController;

                case UITabBarController tabBarController:
                    return tabBarController.SelectedViewController;

                case null:
                    return rootController;

                default:
                    return rootController.PresentedViewController;
            }
        }

    }
}