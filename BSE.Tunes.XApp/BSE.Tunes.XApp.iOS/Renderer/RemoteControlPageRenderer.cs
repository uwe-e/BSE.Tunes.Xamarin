using BSE.Tunes.XApp.Controls;
using BSE.Tunes.XApp.iOS.Renderer;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RemoteControlPage), typeof(RemoteControlPageRenderer))]
namespace BSE.Tunes.XApp.iOS.Renderer
{
    public class RemoteControlPageRenderer : PageRenderer
    {
        private AudioPlayerState _audioPlayerState = AudioPlayerState.Closed;

        RemoteControlPage Page => Element as RemoteControlPage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            Page.PropertyChanged += OnPagePropertyChanged;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Page != null)
                {
                    Page.PropertyChanged -= OnPagePropertyChanged;
                }
            }
            base.Dispose(disposing);
        }

        private void OnPagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Page.AudioPlayerState))
            {
                SetPlayerState(Page.AudioPlayerState);
            }
        }

        public override void RemoteControlReceived(UIEvent theEvent)
        {
            base.RemoteControlReceived(theEvent);

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

        private void SetPlayerState(AudioPlayerState audioPlayerState)
        {
            _audioPlayerState = audioPlayerState;
        }

    }
}