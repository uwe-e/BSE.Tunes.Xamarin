using BSE.Tunes.XApp.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using BSERenderer = BSE.Tunes.XApp.iOS.Renderer;
using ButtonSpecific = BSE.Tunes.XApp.PlatformConfiguration.iOSSpecific.Button;

[assembly: ExportRenderer(typeof(Button), typeof(BSERenderer.ButtonRenderer))]
namespace BSE.Tunes.XApp.iOS.Renderer
{
    public class ButtonRenderer : Xamarin.Forms.Platform.iOS.ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                SetLineBreakMode(e);
                SetTextAlignment(e);
                SetHorizontalTextAlignment(e);
            }
        }

        private void SetHorizontalTextAlignment(ElementChangedEventArgs<Button> e)
        {
            if (e.NewElement is FlyoutButton flyoutButton)
            {
                var horizontalTextAlignment = flyoutButton.HorizontalContentAlignment;
                switch (horizontalTextAlignment)
                {
                    case TextAlignment.Start:
                        Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        break;
                    case TextAlignment.End:
                        Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
                        break;
                }
            }
        }

        private void SetLineBreakMode(ElementChangedEventArgs<Button> e)
        {
            var lineBreakMode = ButtonSpecific.GetUILineBreakMode(e.NewElement);
            switch (lineBreakMode)
            {
                case PlatformConfiguration.iOSSpecific.UILineBreakMode.WordWrap:
                    Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.WordWrap;
                    break;
                case PlatformConfiguration.iOSSpecific.UILineBreakMode.Clip:
                    Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.Clip;
                    break;
                case PlatformConfiguration.iOSSpecific.UILineBreakMode.CharacterWrap:
                    Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.CharacterWrap;
                    break;
                case PlatformConfiguration.iOSSpecific.UILineBreakMode.HeadTruncation:
                    Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.HeadTruncation;
                    break;
                case PlatformConfiguration.iOSSpecific.UILineBreakMode.MiddleTruncation:
                    Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.MiddleTruncation;
                    break;
                case PlatformConfiguration.iOSSpecific.UILineBreakMode.TailTruncation:
                    Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.MiddleTruncation;
                    break;
            }
        }

        private void SetTextAlignment(ElementChangedEventArgs<Button> e)
        {
            var textAlignment = ButtonSpecific.GetUITextAlignment(e.NewElement);
            switch (textAlignment)
            {
                case PlatformConfiguration.iOSSpecific.UITextAlignment.Center:
                    Control.TitleLabel.TextAlignment = UIKit.UITextAlignment.Center;
                    break;
                case PlatformConfiguration.iOSSpecific.UITextAlignment.Right:
                    Control.TitleLabel.TextAlignment = UIKit.UITextAlignment.Right;
                    break;
            }
        }
    }
}