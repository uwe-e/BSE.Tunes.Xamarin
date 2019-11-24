using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IEnvironment
    {
        Theme GetOperatingSystemTheme();
        Task<Theme> GetOperatingSystemThemeAsync();
    }

    public enum Theme { Light, Dark }
}
