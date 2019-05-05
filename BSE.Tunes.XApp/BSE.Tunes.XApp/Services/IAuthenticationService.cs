using BSE.Tunes.XApp.Models.IdentityModel;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IAuthenticationService
    {
        TokenResponse TokenResponse
        {
            get;
        }
        Task<bool> LoginAsync(string userName, string password);
        Task<bool> RequestRefreshTokenAsync(string refreshToken);
    }
}
