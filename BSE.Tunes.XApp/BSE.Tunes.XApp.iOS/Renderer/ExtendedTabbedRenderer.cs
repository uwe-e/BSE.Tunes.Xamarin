using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedTabbedPage), typeof(ExtendedTabbedRenderer))]
namespace BSE.Tunes.XApp.iOS.Renderer
{
    public class ExtendedTabbedRenderer : TabbedRenderer
    {
        private UIView _audioPlayerBar;

        ExtendedTabbedPage Page => Element as ExtendedTabbedPage;

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (Element == null)
                return;

            if (Element.Parent is BaseShellItem)
                Element.Layout(View.Bounds.ToRectangle());

            if (!Element.Bounds.IsEmpty)
            {
                View.Frame = new System.Drawing.RectangleF((float)Element.X, (float)Element.Y, (float)Element.Width, (float)Element.Height);
            }

            var frame = View.Frame;
            var tabBarFrame = TabBar.Frame;
            var audioPlayerFrame = _audioPlayerBar.Frame;

            _audioPlayerBar.Frame = new System.Drawing.RectangleF((float)Element.X, (float)(frame.Top + frame.Height - tabBarFrame.Height - 60), (float)Element.Width, (float)60);

            Page.ContainerArea = new Rectangle(0, 0, frame.Width, frame.Height - audioPlayerFrame.Height - tabBarFrame.Height);

        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Page == null)
            {
                return;
            }
            try
            {
                SetupUserInterface();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"\t\t\tERROR: {exception.Message}");
            }
        }

        private void SetupUserInterface()
        {
            IVisualElementRenderer audioPlayerRenderer = Platform.GetRenderer(Page.AudioPlayerBar);
            if (audioPlayerRenderer == null)
            {
                audioPlayerRenderer = Platform.CreateRenderer(Page.AudioPlayerBar);
                Platform.SetRenderer(Page.AudioPlayerBar, audioPlayerRenderer);
            }
            _audioPlayerBar = audioPlayerRenderer.NativeView;
            _audioPlayerBar.Hidden = false;
            _audioPlayerBar.BackgroundColor = ((TabbedPage)Element).BarBackgroundColor.ToUIColor();

            View.AddSubview(_audioPlayerBar);
            //View.Add(_audioPlayerBar);
        }
    }
}