using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using BSE.Tunes.XApp.Models.Contract;
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
        private UIControl _infoPanel;
        private UIButton _playNextButton;
        private UIButton _playButton;
        private UIView _bottomBorder;
        private UILabel _titleLabel;
        private UILabel _artistLabel;
        private UIImageView _coverImage;
        private UIProgressView _progressView;

        AudioPlayer Player => Element as AudioPlayer;

        protected override void OnElementChanged(ElementChangedEventArgs<AudioPlayer> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                SetupUserInterface();
                if (e.NewElement.ProgressColor != Color.Default)
                {
                    SetProgressColor(e.NewElement.ProgressColor);
                }

                UpdateIconColors();

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
            
            _infoPanel.Frame = new CGRect(leftX,
                                   7,
                                   controlWidth + offsetLeft + rightX - 10 - (2 * controlWidth) - (3 * offsetLeft),
                                   controlHeight);

            // coverimage + both labels are controls within the infopanel
            _coverImage.Frame = new CGRect(0,
                                   0,
                                   controlWidth,
                                   controlHeight);

            _titleLabel.Frame = new CGRect(controlWidth + offsetLeft,
                                   0,
                                   _infoPanel.Frame.Width - controlWidth - offsetLeft - offsetLeft,
                                   24);

            _artistLabel.Frame = new CGRect(controlWidth + offsetLeft,
                                   25,
                                   _infoPanel.Frame.Width - controlWidth - offsetLeft - offsetLeft,
                                   20);

            _playButton.Frame = new CGRect(rightX - controlWidth - offsetLeft,
                                   7,
                                   controlWidth,
                                   controlHeight);

            _playNextButton.Frame = new CGRect(rightX,
                                   7,
                                   controlWidth,
                                   controlHeight);

            _bottomBorder.Frame = new CGRect(Bounds.Left,
                Bounds.Top + Bounds.Height - 1,
                Bounds.Width,
                1);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            System.Diagnostics.Debug.WriteLine($"{nameof(OnElementChanged)} Property: {e.PropertyName}");
            if (e.PropertyName == nameof(Player.Progress))
            {
                SetProgress(Player.Progress);
            }
            if (e.PropertyName == nameof(Player.ProgressColor))
            {
                SetProgressColor(Player.ProgressColor);
            }
            if (e.PropertyName == nameof(Player.AudioPlayerState))
            {
                SetPlayerState(Player.AudioPlayerState);
            }
            if (e.PropertyName == nameof(Player.Track))
            {
                SetTrackInfo(Player.Track);
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

        public override void RemoteControlReceived(UIEvent theEvent)
        {
            // executed from ExtendedTabbedRenderer
            Console.WriteLine($"{nameof(RemoteControlReceived)} {theEvent.Subtype} ");
            switch (theEvent.Subtype)
            {
                case UIEventSubtype.RemoteControlPause:
                    PlayButtonTouchUpInside(this, EventArgs.Empty);
                    break;
                case UIEventSubtype.RemoteControlPlay:
                    PlayButtonTouchUpInside(this, EventArgs.Empty);
                    break;
                case UIEventSubtype.RemoteControlPreviousTrack:
                    PlayPreviousButtonTouchUpInside(this, EventArgs.Empty);
                    break;
                case UIEventSubtype.RemoteControlNextTrack:
                    PlayNextButtonTouchUpInside(this, EventArgs.Empty);
                    break;
            }
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            if (previousTraitCollection?.UserInterfaceStyle != TraitCollection.UserInterfaceStyle)
            {
                UpdateIconColors();
            }
        }

        private void SetupUserInterface()
        {
            _progressView = new UIProgressView()
            {
                Progress = (float)Player.Progress
            };
            AddSubview(_progressView);

            _infoPanel = new UIControl();
            _infoPanel.TouchUpInside -= InfoPanelTouchUpInside;
            _infoPanel.TouchUpInside += InfoPanelTouchUpInside;
            AddSubview(_infoPanel);

            _coverImage = new UIImageView();
            _infoPanel.AddSubview(_coverImage);

            _titleLabel = new UILabel()
            {
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _infoPanel.AddSubview(_titleLabel);

            _artistLabel = new UILabel()
            {
                LineBreakMode = UILineBreakMode.TailTruncation,
                Font = UIFont.SystemFontOfSize(11)
            };
            _infoPanel.AddSubview(_artistLabel);

            _playButton = new UIButton();
            _playButton.TouchUpInside -= PlayButtonTouchUpInside;
            _playButton.TouchUpInside += PlayButtonTouchUpInside;
            AddSubview(_playButton);

            _playNextButton = new UIButton();
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

        private void UpdateIconColors()
        {
            if (this.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark)
            {
                _playButton?.SetBackgroundImage(UIImage.FromFile("icon_play_wh_20"), UIControlState.Normal);
                if (_audioPlayerState == AudioPlayerState.Playing)
                {
                    _playButton?.SetBackgroundImage(UIImage.FromFile("icon_pause_wh_20"), UIControlState.Normal);
                }
                
                _playNextButton?.SetBackgroundImage(UIImage.FromFile("icon_playnext_wh"), UIControlState.Normal);
                _playNextButton?.SetBackgroundImage(UIImage.FromFile("icon_playnext_gy_dark"), UIControlState.Disabled);
            }
            else
            {
                _playButton?.SetBackgroundImage(UIImage.FromFile("icon_play_bk_20"), UIControlState.Normal);
                if(_audioPlayerState == AudioPlayerState.Playing)
                {
                    _playButton?.SetBackgroundImage(UIImage.FromFile("icon_pause_bk_20"), UIControlState.Normal);
                }

                _playNextButton?.SetBackgroundImage(UIImage.FromFile("icon_playnext_bk"), UIControlState.Normal);
                _playNextButton?.SetBackgroundImage(UIImage.FromFile("icon_playnext_gy_light"), UIControlState.Disabled);
            }
        }
        private void SetProgressColor(Color progressColor)
        {
            _progressView.ProgressTintColor = progressColor.ToUIColor();
        }

        private void SetProgress(double progress)
        {
            _progressView.Hidden = progress == default ? true : false;
            _progressView.SetProgress((float)progress, progress == default ? false : true);
        }
        private void SetTrackInfo(Track track)
        {
            _titleLabel.Text = track?.Name;
            _artistLabel.Text = track?.Album?.Artist?.Name;
        }
        private async void SetCover(ImageSource imageSource)
        {
            var image = await Task.Run(() => new ImageLoaderSourceHandler().LoadImageAsync(imageSource, default, 1f));
            if (image != null)
            {
                _coverImage.Image = image;
            }
        }

        private void SetPlayerState(AudioPlayerState audioPlayerState)
        {
            _audioPlayerState = audioPlayerState;
            
            var iconPlay = "icon_play_bk_20";
            var iconPause = "icon_pause_bk_20";
            if (this.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark)
            {
                iconPlay = "icon_play_wh_20";
                iconPause = "icon_pause_wh_20";
            }

            switch (audioPlayerState)
            {
                case AudioPlayerState.Playing:
                    _playButton.SetBackgroundImage(UIImage.FromFile(iconPause), UIControlState.Normal);
                    break;
                case AudioPlayerState.Paused:
                    _playButton.SetBackgroundImage(UIImage.FromFile(iconPlay), UIControlState.Normal);
                    break;
            }
        }

        private void InfoPanelTouchUpInside(object sender, EventArgs e)
        {
            OnCoverImageButtonTouchUpInside(Element as IPlayerController);
        }

        private void OnCoverImageButtonTouchUpInside(IPlayerController playerController)
        {
            playerController.SendSelectTrackClicked();
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

        private void PlayPreviousButtonTouchUpInside(object sender, EventArgs e)
        {
            OnPlayPreviousButtonTouchUpInside(Element as IPlayerController);
        }

        private void OnPlayPreviousButtonTouchUpInside(IPlayerController element)
        {
            element?.SendPlayPreviousClicked();
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