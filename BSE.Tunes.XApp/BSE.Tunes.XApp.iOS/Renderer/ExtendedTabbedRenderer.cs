using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using System;
using System.ComponentModel;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
        }

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
            if (_audioPlayerBar != null)
            {
                _audioPlayerBar.Frame = new System.Drawing.RectangleF((float)Element.X, (float)(frame.Top + frame.Height - tabBarFrame.Height - 60), (float)Element.Width, (float)60);
                
                var audioPlayerFrame = _audioPlayerBar.Frame;
                
                Page.ContainerArea = new Rectangle(0, 0, frame.Width, frame.Height - audioPlayerFrame.Height - tabBarFrame.Height);
            }
        }

        public override void RemoteControlReceived(UIEvent theEvent)
        {
            base.RemoteControlReceived(theEvent);
            // The AudioPlayer view does not receive a RemoteControlReceived event.
            // Because of this we execute that event from here.
            //Console.WriteLine($"{nameof(RemoteControlReceived)} {theEvent.Subtype} ");
            _audioPlayerBar?.RemoteControlReceived(theEvent);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Tabbed != null)
                {
                    Tabbed.PropertyChanged -= OnPropertyChanged;
                }
            }
            base.Dispose(disposing);
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
                Tabbed.PropertyChanged += OnPropertyChanged;
                SetupUserInterface();
                UpdatePlayerBackgroundColor();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"\t\t\tERROR: {exception.Message}");
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
            {
                UpdatePlayerBackgroundColor();
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

            View.AddSubview(_audioPlayerBar);
        }
        
        private void UpdatePlayerBackgroundColor()
        {
            _audioPlayerBar.BackgroundColor = ((TabbedPage)Element).BarBackgroundColor.ToUIColor();
        }
    }
}