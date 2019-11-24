using BSE.Tunes.XApp.Services;
using BSE.Tunes.XApp.UWP.Services;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Application = Windows.UI.Xaml.Application;

[assembly: Dependency(typeof(Environment_UWP))]
namespace BSE.Tunes.XApp.UWP.Services
{
    public class Environment_UWP : IEnvironment
    {
        public Task<Theme> GetOperatingSystemThemeAsync() => Task.FromResult(GetOperatingSystemTheme());

        public Theme GetOperatingSystemTheme()
        {
            var isDark = Application.Current.RequestedTheme == ApplicationTheme.Dark;
            return isDark ? Theme.Dark : Theme.Light;

        }
    }
}
