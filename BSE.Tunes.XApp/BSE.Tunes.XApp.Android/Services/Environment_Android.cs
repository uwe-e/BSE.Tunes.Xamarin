using Android.Content.Res;
using Android.OS;
using BSE.Tunes.XApp.Droid.Services;
using BSE.Tunes.XApp.Services;
using Plugin.CurrentActivity;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Environment_Android))]
namespace BSE.Tunes.XApp.Droid.Services
{
    public class Environment_Android : IEnvironment
    {
        public Task<Theme> GetOperatingSystemThemeAsync() => Task.FromResult(GetOperatingSystemTheme());
        
        public Theme GetOperatingSystemTheme()
        {

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Froyo)
            {
                // requires the Plugin.CurrentActivity NuGet Package
                var uiModeFlags = CrossCurrentActivity.Current.AppContext.Resources.Configuration.UiMode & UiMode.NightMask;

                switch (uiModeFlags)
                {
                    case UiMode.NightYes:
                        return Theme.Dark;
                    default:
                        return Theme.Light;

                }
            }
            return Theme.Light;
        }
    }
}