using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using CoreGraphics;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
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
        private UIView _bottomBorder;
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
            var controlHeight = 46;
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

            _bottomBorder.Frame = new CGRect(Bounds.Left,
                Bounds.Top + Bounds.Height -1 ,
                Bounds.Width,
                1);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Player.Progress))
            {
                SetProgress(Player.Progress);
            }
            if (e.PropertyName == nameof(Player.AudioPlayerState))
            {
                SetPlayerState(Player.AudioPlayerState);
            }
            if (e.PropertyName == nameof(Player.TrackTitle))
            {
                SetTrackTitle(Player.TrackTitle);
            }
            if (e.PropertyName == nameof(Player.Cover))
            {
                SetCover(Player.Cover);
            }
            if (e.PropertyName == nameof(Player.IsPlayNextEnabled))
            {
                UpdateIsPlayNextEnabled(Player.IsPlayNextEnabled);
            }
        }

        private void SetupUserInterface()
        {
            _progressView = new UIProgressView()
            {
                //BackgroundColor = UIColor.Red,
                Progress = (float)Player.Progress
            };

            AddSubview(_progressView);

            _coverImageView = new UIImageView()
            {
                //BackgroundColor = UIColor.Orange,
                //Image = UIImage.f
            };

            AddSubview(_coverImageView);

            _trackTitleLabel = new UILabel()
            {
                //BackgroundColor = UIColor.Blue,
                LineBreakMode = UILineBreakMode.TailTruncation,
                //Text = "This is the title"
            };

            AddSubview(_trackTitleLabel);

            _playButton = new UIButton()
            {
                //BackgroundColor = UIColor.Green,
            };
            _playButton.SetBackgroundImage(UIImage.FromFile("icon_play_d_blue_20_50"), UIControlState.Normal);
            _playButton.TouchUpInside -= PlayButtonTouchUpInside;
            _playButton.TouchUpInside += PlayButtonTouchUpInside;

            AddSubview(_playButton);

            _playNextButton = new UIButton()
            {
                //BackgroundColor = UIColor.Yellow,
            };
            _playNextButton.SetBackgroundImage(UIImage.FromFile("icon-playnext_gray_20_50"), UIControlState.Disabled);
            _playNextButton.SetBackgroundImage(UIImage.FromFile("icon-playnext_d_blue_20_50"), UIControlState.Normal);
            _playNextButton.TouchUpInside -= PlayNextButtonTouchUpInside;
            _playNextButton.TouchUpInside += PlayNextButtonTouchUpInside;
            _playNextButton.Enabled = false;
            AddSubview(_playNextButton);


            _bottomBorder = new UIView
            {
                BackgroundColor = UIColor.SeparatorColor
            };
            AddSubview(_bottomBorder);
        }
        private void SetProgress(double progress)
        {
            _progressView.Hidden = progress == default ? true : false;
            _progressView.SetProgress((float)progress, progress == default ? false : true);
        }
        private void SetTrackTitle(string trackTitle)
        {
            _trackTitleLabel.Text = trackTitle;
        }

        private async void SetCover(ImageSource imageSource)
        {
            var image = await Task.Run(() => new ImageLoaderSourceHandler().LoadImageAsync(imageSource, default, 1f));
            if (image != null)
            {
                _coverImageView.Image = image;
            }
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

        private void UpdateIsPlayNextEnabled(bool isPlayNextEnabled)
        {
            _playNextButton.Enabled = isPlayNextEnabled;
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