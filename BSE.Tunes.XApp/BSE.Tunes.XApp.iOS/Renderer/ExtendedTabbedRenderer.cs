using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedTabbedPage), typeof(ExtendedTabbedRenderer))]
namespace BSE.Tunes.XApp.iOS.Renderer
{
    public class ExtendedTabbedRenderer : TabbedRenderer
    {
        UIButton _playButton;
        UIButton _playNextButton;
        UIView _playerBar;
        Page Page => Element as Page;
        
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            
            if (e.OldElement != null || Element == null)
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
            var playerFrame = _playerBar.Frame;
            
            _playerBar.Frame = new System.Drawing.RectangleF((float)Element.X, (float)(frame.Top + frame.Height - tabBarFrame.Height - 50), (float)Element.Width, (float)50);
            
            Page.ContainerArea = new Rectangle(0, 0, frame.Width, frame.Height - playerFrame.Height - tabBarFrame.Height);

        }

        private void SetupUserInterface()
        {
            var rightX = View.Bounds.Right - 47;
            
            _playButton = new UIButton()
            {
                Frame = new CGRect(rightX - 47, 7, 33, 33)
            };
            _playButton.SetBackgroundImage(UIImage.FromFile("icon_play_blue@2x.png"), UIControlState.Normal);

            _playNextButton = new UIButton()
            {
                Frame = new CGRect(rightX, 7, 33, 33)
            };
            _playNextButton.SetBackgroundImage(UIImage.FromFile("icon_playnext_blue@2x.png"), UIControlState.Normal);
            
            _playerBar = new UIView();
            _playerBar.BackgroundColor = ((TabbedPage)Element).BarBackgroundColor.ToUIColor();
            _playerBar.Add(_playButton);
            _playerBar.Add(_playNextButton);

            View.Add(_playerBar);
           
        }
    }
}