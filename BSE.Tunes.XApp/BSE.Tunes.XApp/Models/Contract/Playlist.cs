﻿using System;
using System.Collections.Generic;

namespace BSE.Tunes.XApp.Models.Contract
{
    public class Playlist
    {
        private IList<PlaylistEntry> _playlistEntries;

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

        public IList<PlaylistEntry> Entries => _playlistEntries ?? (_playlistEntries = new List<PlaylistEntry>());
    }
}
