using BSE.Tunes.XApp.Models;

namespace BSE.Tunes.XApp.Services
{
    public interface ISettingsService
    {
        User User
        {
            get; set;
        }

        string ServiceEndPoint
        {
            get; set;
        }
    }
}
