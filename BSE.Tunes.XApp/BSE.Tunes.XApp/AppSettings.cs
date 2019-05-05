using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Utils;
using Xamarin.Essentials;

namespace BSE.Tunes.XApp
{
    public static class AppSettings
    {
        public static string ServiceEndPoint
        {
            get => Preferences.Get(nameof(ServiceEndPoint), null);
            set => Preferences.Set(nameof(ServiceEndPoint), value);
        }

        public static User User
        {
            get => PreferencesHelpers.Get(nameof(User), default(User));
            set => PreferencesHelpers.Set(nameof(User), value);
        }
    }
}
