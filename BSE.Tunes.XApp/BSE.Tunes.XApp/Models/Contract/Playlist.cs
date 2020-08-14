using System;
using System.Collections.Generic;

namespace BSE.Tunes.XApp.Models.Contract
{
    public class Playlist
    {
        private IEnumerable<PlaylistEntry> _playlistEntries;

        public int Id
        {
            get; set;
        }
        public Guid Guid
        {
            get;
            set;
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

        public IEnumerable<PlaylistEntry> Entries => _playlistEntries ?? (_playlistEntries = new List<PlaylistEntry>());
    }
}
