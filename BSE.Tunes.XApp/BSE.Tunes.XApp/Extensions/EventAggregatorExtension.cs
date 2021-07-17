using BSE.Tunes.XApp.Events;
using BSE.Tunes.XApp.Models;
using BSE.Tunes.XApp.Models.Contract;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Tunes.XApp.Extensions
{
    public static class EventAggregatorExtension
    {
        public static SubscriptionToken ShowAlbum(this PubSubEvent<Track> pubSubEvent, Action<Track> action)
        {
            return pubSubEvent.Subscribe(action);
        }
    }
}
