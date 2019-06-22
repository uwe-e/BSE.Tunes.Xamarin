using Android.Content;
using Android.Widget;
using BSE.Tunes.XApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HorizontalScrollView), typeof(HorizontalScrollViewRenderer))]
namespace BSE.Tunes.XApp.Droid.Renderers
{
    public class HorizontalScrollViewRenderer : ScrollViewRenderer
    {
        public HorizontalScrollViewRenderer(Context context) : base(context)
        {
        }
    }
}