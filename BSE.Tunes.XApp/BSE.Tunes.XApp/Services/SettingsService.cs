using BSE.Tunes.XApp.Models;

namespace BSE.Tunes.XApp.Services
{
    public class SettingsService : ISettingsService
    {
        public string ServiceEndPoint
        {
            get => AppSettings.ServiceEndPoint;
            set => AppSettings.ServiceEndPoint = value;
        }

        public User User
        {
            get => AppSettings.User;
            set => AppSettings.User = value;
        }

        public string Token
        {
            get => User.Token;
            set
            {
                var user = User;
                if (user != null)
                {
                    user.Token = value;
                    User = user;
                }
            }
        }
    }
}
