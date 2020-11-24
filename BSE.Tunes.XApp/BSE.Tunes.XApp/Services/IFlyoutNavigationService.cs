using Prism.Navigation;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IFlyoutNavigationService : INavigationService
    {
        Task<INavigationResult> ShowFlyoutAsync(string name, INavigationParameters parameters);

        Task<INavigationResult> CloseFlyoutAsync();
    }
}
