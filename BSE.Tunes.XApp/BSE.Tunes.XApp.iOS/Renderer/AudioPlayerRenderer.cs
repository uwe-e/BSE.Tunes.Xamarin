using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using CoreGraphics;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AudioPlayer),
                          typeof(AudioPlayerRenderer))]
namespace BSE.Tunes.XApp.iOS.Renderer
{
    public class AudioPlayerRenderer : ViewRenderer<AudioPlayer, UIView>
    {
        private AudioPlayerState _audioPlayerState = AudioPlayerState.Closed;
        private UIButton _playNextButton;
        private UIButton _playButton;
        private UILabel _trackTitleLabel;
        private UIImageView _coverImageView;
        private UIProgressView _progressView;

        AudioPlayer Player => Element as AudioPlayer;

        protected override void OnElementChanged(ElementChangedEventArgs<AudioPlayer> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                SetupUserInterface();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var leftX = Bounds.Left + 10;
            var rightX = Bounds.Right - 46;
            var offsetLeft = 5;
            var controlHeight = 36;
            var controlWidth = controlHeight;

            _progressView.Frame = new CGRect(Bounds.Left + 2,
                                   Bounds.Top,
                                   Bounds.Width - 4,
                                   7);
            
            _coverImageView.Frame = new CGRect(leftX,
                                   7,
                                   controlWidth,
                                   controlHeight);

            _trackTitleLabel.Frame = new CGRect(leftX + controlWidth + offsetLeft,
                                   7,
                                   rightX - 10 - (2 * controlWidth) - (3 * offsetLeft),
                                   controlHeight);

            _playButton.Frame = new CGRect(rightX - controlWidth - offsetLeft,
                                   7,
                                   controlWidth,
                                   controlHeight);

            _playNextButton.Frame = new CGRect(rightX,
                                   7,
                                   controlWidth,
                                   controlHeight);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Player.Progress))
            {
                _progressView.SetProgress((float)Player.Progress, Player.Progress == default ? false : true);
            }
            if (e.PropertyName == nameof(Player.AudioPlayerState))
            {
                SetPlayerState(Player.AudioPlayerState);
            }
        }

        private void SetupUserInterface()
        {
            _progressView = new UIProgressView()
            {
                BackgroundColor = UIColor.Red,
                Progress = (float)Player.Progress
            };

            AddSubview(_progressView);

            _coverImageView = new UIImageView()
            {
                BackgroundColor = UIColor.Orange,
            };

            AddSubview(_coverImageView);

            _trackTitleLabel = new UILabel()
            {
                BackgroundColor = UIColor.Blue,
                Text = "This is the title"
            };

            AddSubview(_trackTitleLabel);

            _playButton = new UIButton()
            {
                BackgroundColor = UIColor.Green,
            };
            _playButton.SetBackgroundImage(UIImage.FromFile("icon_play_d_blue_20_50.png"), UIControlState.Normal);
            _playButton.TouchUpInside -= PlayButtonTouchUpInside;
            _playButton.TouchUpInside += PlayButtonTouchUpInside;

            AddSubview(_playButton);

            _playNextButton = new UIButton()
            {
                BackgroundColor = UIColor.Yellow,
            };
            _playNextButton.SetBackgroundImage(UIImage.FromFile("icon_playnext_gray@2x.png"), UIControlState.Disabled);
            _playNextButton.SetBackgroundImage(UIImage.FromFile("icon-playnext_d_blue_20_50@2x.png"), UIControlState.Normal);
            _playNextButton.TouchUpInside -= PlayNextButtonTouchUpInside;
            _playNextButton.TouchUpInside += PlayNextButtonTouchUpInside;

            AddSubview(_playNextButton);
        }

        

        private void SetPlayerState(AudioPlayerState audioPlayerState)
        {
            _audioPlayerState = audioPlayerState;
            switch (audioPlayerState)
            {
                case AudioPlayerState.Playing:
                    _playButton.SetBackgroundImage(UIImage.FromFile("icon_pause_d_blue_20_50@2x.png"), UIControlState.Normal);
                    break;
                case AudioPlayerState.Paused:
                    _playButton.SetBackgroundImage(UIImage.FromFile("icon_play_d_blue_20_50@2x.png"), UIControlState.Normal);
                    break;
                //default:
                //    break;
            }
        }
        
        private void PlayButtonTouchUpInside(object sender, EventArgs e)
        {
            OnPlayButtonTouchUpInside(Element as IPlayerController);
        }

        private void OnPlayButtonTouchUpInside(IPlayerController element)
        {
            if (_audioPlayerState == AudioPlayerState.Playing)
            {
                element?.SendPauseClicked();
                return;
            }
            element?.SendPlayClicked();
        }
        
        private void PlayNextButtonTouchUpInside(object sender, EventArgs e)
        {
            OnPlayNextButtonTouchUpInside(Element as IPlayerController);
        }

        private void OnPlayNextButtonTouchUpInside(IPlayerController element)
        {
            element?.SendPlayNextClicked();
        }
    }
}