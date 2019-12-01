
using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.UWP.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(SectionHeaderButton), typeof(SectionHeaderButtonRenderer))]
namespace BSE.Tunes.XApp.UWP.Renderer
{
    public class SectionHeaderButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null)
            {
                this.Control.Style = Windows.UI.Xaml.Application.Current.Resources["SectionHeaderButtonStyle"] as Windows.UI.Xaml.Style;
                this.Control.PointerEntered += new Windows.UI.Xaml.Input.PointerEventHandler((sender, pointerRoutedEventArgs) =>{
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
                });
                this.Control.PointerExited += new Windows.UI.Xaml.Input.PointerEventHandler((sender, pointerRoutedEventArgs) => {
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                });
            }
        }
    }
}
