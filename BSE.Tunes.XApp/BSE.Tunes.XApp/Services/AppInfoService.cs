using Xamarin.Essentials;

namespace BSE.Tunes.XApp.Services
{
    public class AppInfoService : IAppInfoService
    {
        public string VersionString => AppInfo.VersionString;
    }
}
