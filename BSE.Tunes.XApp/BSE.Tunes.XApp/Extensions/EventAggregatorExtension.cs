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
        public static SubscriptionToken ShowA(this PubSubEvent pubSubEvent, Action action)
        {
            return pubSubEvent.Subscribe(action);
        }


        //public static TEventType ShowB(this EventBase pubSubEvent, Action action)
        //{
        //    return pubSubEvent.s.ShowA(action);
        //}



        public static void ShowAlbum<TEvent>(this IEventAggregator eventAggregator, Action<Track> action) where TEvent : PubSubEvent, new()
        {
            eventAggregator.GetEvent<PlaylistActionContextChanged>().Subscribe(args =>
            {
                if (args is PlaylistActionContext managePlaylistContext)
                {
                    if (managePlaylistContext.ActionMode == PlaylistActionMode.ShowAlbum)
                    {
                        if (managePlaylistContext.Data is Track track)
                        {
                            action.Invoke(track);
                        }
                    }
                }
            }, ThreadOption.UIThread);
        }
    }
}
