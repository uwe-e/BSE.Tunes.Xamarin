using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.XApp.Services
{
    public interface IStreamingService
    {
        Task<long> GetContentLength(Guid guid);

        Task<System.IO.Stream> GetStream(Guid guid);

        Task<int> GetPartialContent(Guid guid, long rangeFrom, long rangeTo);

        Task<System.IO.Stream> GetPartialStream(Guid guid, long rangeFrom, long rangeTo);
    }
}
