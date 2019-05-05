using BSE.Tunes.XApp.Models;

namespace BSE.Tunes.XApp.Services
{
    public class SettingsService : ISettingsService
    {
        public string ServiceEndPoint
        {
            get
            {
                return AppSettings.ServiceEndPoint;
            }
            set
            {
                AppSettings.ServiceEndPoint = value;
            }
        }

        public User User
        {
            get
            {
                return AppSettings.User;
            }
            set
            {
                AppSettings.User = value;
            }
        }
    }
}
