using System;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IRequestService
    {
        Task<T> GetAsync<T>(Uri uri);
        Task<U> PostAsync<T, U>(Uri uri, U from);
    }
}
