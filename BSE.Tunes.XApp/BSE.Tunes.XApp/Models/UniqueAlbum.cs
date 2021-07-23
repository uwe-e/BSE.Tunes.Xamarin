using BSE.Tunes.XApp.Models.Contract;
using System;

namespace BSE.Tunes.XApp.Models
{
    public class UniqueAlbum
    {
        public Guid UniqueId
        {
            get;set;
        }

        public Album Album
        {
            get;set;
        }
    }
}
