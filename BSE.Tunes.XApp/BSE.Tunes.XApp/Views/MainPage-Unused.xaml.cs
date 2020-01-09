using Xamarin.Forms;

namespace BSE.Tunes.XApp.Views
{
    public partial class MainPageUnused : MasterDetailPage
    {
        public MainPageUnused()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }
        }
    }
}