namespace BSE.Tunes.XApp.Models.Contract
{
    public class Artist
    {
        public int Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string SortName
        {
            get; set;
        }

        public Album[] Albums
        {
            get; set;
        }
    }
}
