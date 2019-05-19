﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp.Models.Contract
{
    public class Album
    {
        public int Id
        {
            get; set;
        }

        public Artist Artist
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public int? Year
        {
            get; set;
        }

        public byte[] Thumbnail
        {
            get; set;
        }

        public byte[] Cover
        {
            get; set;
        }
        public Genre Genre

        {
            get; set;
        }

        public Track[] Tracks
        {
            get; set;
        }

        public Guid AlbumId
        {
            get; set;
        }
    }
}
