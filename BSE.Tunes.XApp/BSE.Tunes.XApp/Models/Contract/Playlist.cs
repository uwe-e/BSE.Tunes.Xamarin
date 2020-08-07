﻿using System;

namespace BSE.Tunes.XApp.Models.Contract
{
    public class Playlist
    {
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
    }
}
