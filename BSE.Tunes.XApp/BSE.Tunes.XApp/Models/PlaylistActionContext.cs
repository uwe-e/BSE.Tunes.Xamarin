using BSE.Tunes.XApp.Models.Contract;

namespace BSE.Tunes.XApp.Models
{
    public class PlaylistActionContext
    {
        public PlaylistActionMode ActionMode { get; set; }
        
        public Playlist PlaylistTo { get; set; }
        
        public object Data { get; set; }
    }
}
