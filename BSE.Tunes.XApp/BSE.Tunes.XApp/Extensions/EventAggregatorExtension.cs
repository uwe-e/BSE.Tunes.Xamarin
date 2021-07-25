using BSE.Tunes.XApp.Models;
using Prism.Events;
using System;

namespace BSE.Tunes.XApp.Extensions
{
    public static class EventAggregatorExtension
    {
        public static SubscriptionToken ShowAlbum(this PubSubEvent<UniqueAlbum> pubSubEvent, Action<UniqueAlbum> action)
        {
            return pubSubEvent.Subscribe(action,ThreadOption.UIThread);
        }

    }
}
