using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace BSE.Tunes.XApp.Views
{
    public partial class PlaylistSelectorDialogPage : ContentPage
    {
        public PlaylistSelectorDialogPage()
        {
            InitializeComponent();

            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
        }
    }
}
