using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp.Models.Contract
{
    public class Playlist
    {
        public int Id
        {
            get; set;
        }
        public string Name
        {
            get;
            set;
        }
        public int NumberEntries
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }
    }
}
