﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp.Models.Contract
{
    public class History
    {
        public int PlayMode
        {
            get;
            set;
        }
        public int AlbumId
        {
            get;
            set;
        }
        public int TrackId
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }
        public DateTime PlayedAt
        {
            get;
            set;
        }
    }
}
