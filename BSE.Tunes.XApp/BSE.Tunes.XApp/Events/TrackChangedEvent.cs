﻿using BSE.Tunes.XApp.Models.Contract;
using Prism.Events;

namespace BSE.Tunes.XApp.Events
{
    public class TrackChangedEvent : PubSubEvent<Track>
    {
    }
}
