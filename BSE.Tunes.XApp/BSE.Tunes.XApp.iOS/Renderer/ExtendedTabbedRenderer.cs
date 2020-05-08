using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using BSE.Tunes.XApp.Services;
using CoreGraphics;
using System;
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
        UIProgressView _progressView;

        AudioPlayerState CurrentAudioPlayerState { get; set; } = AudioPlayerState.Closed;
        ExtendedTabbedPage Page => Element as ExtendedTabbedPage;
        IDataService DataService => DependencyService.Resolve<IDataService>();

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
                Page.PropertyChanged += OnPropertyChanged;
                Page.PlayStateChanged += OnPlayStateChanged;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"\t\t\tERROR: {exception.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            Page.PropertyChanged -= OnPropertyChanged;
            base.Dispose(disposing);
        }

        private void SetupUserInterface()
        {
            var rightX = View.Bounds.Right - 47;

            _progressView = new UIProgressView()
            {
                Frame = new CGRect(View.Bounds.Left + 2, View.Bounds.Top, View.Bounds.Width - 4, 7),
                Progress = (float)Page.Progress
            };

            _playButton = new UIButton()
            {
                Frame = new CGRect(rightX - 47, 7, 33, 33)
            };
            _playButton.SetBackgroundImage(UIImage.FromFile("icon_play_blue@2x.png"), UIControlState.Normal);
            _playButton.TouchUpInside -= PlayButtonTouchUpInside;
            _playButton.TouchUpInside += PlayButtonTouchUpInside;

            _playNextButton = new UIButton()
            {
                Frame = new CGRect(rightX, 7, 33, 33)
            };
            _playNextButton.SetBackgroundImage(UIImage.FromFile("icon_playnext_gray@2x.png"), UIControlState.Disabled);
            _playNextButton.SetBackgroundImage(UIImage.FromFile("icon_playnext_blue@2x.png"), UIControlState.Normal);
            _playNextButton.TouchUpInside -= PlayNextButtonTouchUpInside;
            _playNextButton.TouchUpInside += PlayNextButtonTouchUpInside;

            _playerBar = new UIView
            {
                BackgroundColor = ((TabbedPage)Element).BarBackgroundColor.ToUIColor()
            };
            _playerBar.Add(_progressView);
            _playerBar.Add(_playButton);
            _playerBar.Add(_playNextButton);

            View.Add(_playerBar);

        }
        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Page.Progress))
            {
                _progressView.SetProgress((float)Page.Progress, Page.Progress == default ? false : true);
            }
        }

        private void OnPlayStateChanged(object sender, PlayStateChangedEventArgs args)
        {
            var audioPlayerState = args.NewState;
            switch (audioPlayerState)
            {
                case AudioPlayerState.Playing:
                    _playButton.SetBackgroundImage(UIImage.FromFile("icon_pause_blue@2x.png"), UIControlState.Normal);
                    break;
                case AudioPlayerState.Paused:
                    _playButton.SetBackgroundImage(UIImage.FromFile("icon_play_blue@2x.png"), UIControlState.Normal);
                    break;
            }
            CurrentAudioPlayerState = audioPlayerState;
        }

        private void PlayButtonTouchUpInside(object sender, EventArgs e)
        {
            OnPlayButtonTouchUpInside(Element as IPlayerController, CurrentAudioPlayerState);
        }

        internal static void OnPlayButtonTouchUpInside(IPlayerController element, AudioPlayerState playState)
        {
            if (playState == AudioPlayerState.Playing)
            {
                element?.SendPauseClicked();
                return;
            }
            element?.SendPlayClicked();
        }

        private void PlayNextButtonTouchUpInside(object sender, EventArgs e)
        {
            OnPlayNextButtonTouchUpInside(Element as IPlayerController, CurrentAudioPlayerState);
        }

        internal static void OnPlayNextButtonTouchUpInside(IPlayerController element, AudioPlayerState playState)
        {
            //if (playState == PlayState.Playing)
            //{
            //    element?.SendPauseClicked();
            //    return;
            //}
            element?.SendPlayNextClicked();
        }
    }
}