using Android.Content;
using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.Droid.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(FlyoutButton), typeof(ButtonRenderer))]
namespace BSE.Tunes.XApp.Droid.Renderers
{

    public class ButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
    {
        public ButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                SetHorizontalTextAlignment(e);
            }
        }

        private void SetHorizontalTextAlignment(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Button> e)
        { 
            if (e.NewElement is FlyoutButton flyoutButton)
            {
                var horizontalTextAlignment = flyoutButton.HorizontalContentAlignment;
                switch (horizontalTextAlignment)
                {
                    case Xamarin.Forms.TextAlignment.Start:
                        Control.CompoundDrawablePadding = 50;
                        Control.Gravity = Android.Views.GravityFlags.CenterVertical | Android.Views.GravityFlags.Left;
                        break;
                }
            }
        }
    }
}