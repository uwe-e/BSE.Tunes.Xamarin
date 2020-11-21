using BSE.Tunes.XApp.Models.Contract;
using Prism.Events;

namespace BSE.Tunes.XApp.Events
{
    public class AddPlaylistEntryToPlaylistEvent : PubSubEvent<PlaylistEntry>
    {
    }
}
