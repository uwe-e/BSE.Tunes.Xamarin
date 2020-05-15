using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BSE.Tunes.XApp.Controls
{
    public class AudioPlayer : View, IPlayerController
    {
        public static readonly BindableProperty PlayCommandProperty
        = BindableProperty.Create(
            nameof(PlayCommand),
            typeof(ICommand),
            typeof(AudioPlayer),
            null,
            propertyChanging: OnPlayCommandChanging,
            propertyChanged: OnPlayCommandChanged);

        public ICommand PlayCommand
        {
            get { return (ICommand)GetValue(PlayCommandProperty); }
            set { SetValue(PlayCommandProperty, value); }
        }

        public static readonly BindableProperty PlayCommandParameterProperty
            = BindableProperty.Create(
                nameof(PlayCommandParameter),
                typeof(object),
                typeof(AudioPlayer),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PlayCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public object PlayCommandParameter
        {
            get { return GetValue(PlayCommandParameterProperty); }
            set { SetValue(PlayCommandParameterProperty, value); }
        }

        public static readonly BindableProperty PauseCommandProperty
            = BindableProperty.Create(
                nameof(PauseCommand),
                typeof(ICommand),
                typeof(AudioPlayer),
                null,
                propertyChanging: OnPauseCommandChanging,
                propertyChanged: OnPauseCommandChanged);

        public ICommand PauseCommand
        {
            get { return (ICommand)GetValue(PauseCommandProperty); }
            set { SetValue(PauseCommandProperty, value); }
        }

        public static readonly BindableProperty PauseCommandParameterProperty
            = BindableProperty.Create(
                nameof(PauseCommandParameter),
                typeof(object),
                typeof(AudioPlayer),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PauseCommandCanExecuteChanged(bindable, EventArgs.Empty));
        
        public object PauseCommandParameter
        {
            get { return GetValue(PauseCommandParameterProperty); }
            set { SetValue(PauseCommandParameterProperty, value); }
        }

        public static readonly BindableProperty PlayNextCommandProperty
           = BindableProperty.Create(
               nameof(PlayNextCommand),
               typeof(ICommand),
               typeof(AudioPlayer),
               null,
               propertyChanging: OnPlayNextCommandChanging,
               propertyChanged: OnPlayNextCommandChanged);

        public ICommand PlayNextCommand
        {
            get { return (ICommand)GetValue(PlayNextCommandProperty); }
            set { SetValue(PlayNextCommandProperty, value); }
        }

        public static readonly BindableProperty PlayNextCommandParameterProperty
            = BindableProperty.Create(
                nameof(PlayNextCommandParameter),
                typeof(object),
                typeof(AudioPlayer),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PlayNextCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public object PlayNextCommandParameter
        {
            get { return GetValue(PlayNextCommandParameterProperty); }
            set { SetValue(PlayNextCommandParameterProperty, value); }
        }

        public static readonly BindableProperty ProgressProperty
                  = BindableProperty.Create(
                      nameof(Progress),
                      typeof(double),
                      typeof(AudioPlayer), 0d, coerceValue: (bo, v) => ((double)v).Clamp(0, 1));
        
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly BindableProperty AudioPlayerStateProperty
                    = BindableProperty.Create(
                        nameof(AudioPlayerState),
                        typeof(AudioPlayerState),
                        typeof(AudioPlayer),
                        AudioPlayerState.Closed);

        public AudioPlayerState AudioPlayerState
        {
            get { return (AudioPlayerState)GetValue(AudioPlayerStateProperty); }
            set { SetValue(AudioPlayerStateProperty, value); }
        }

        public void SendPauseClicked()
        {
            PauseCommand?.Execute(PauseCommandParameter);
        }

        public void SendPlayClicked()
        {
            PlayCommand?.Execute(PlayCommandParameter);
        }

        public void SendPlayNextClicked()
        {
            PlayNextCommand?.Execute(PlayNextCommandParameter);
        }
        
        private static void OnPlayCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AudioPlayer control)
            {
                if (newValue is ICommand newCommand)
                {
                    newCommand.CanExecuteChanged += PlayCommandCanExecuteChanged;
                }
            }
        }

        private static void PlayCommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (sender is AudioPlayer control)
            {
                control.PlayCommand?.CanExecute(control.PlayCommandParameter);
            }
        }

        private static void OnPlayCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AudioPlayer control)
            {
                if (oldValue != null)
                {
                    (oldValue as ICommand).CanExecuteChanged -= PlayCommandCanExecuteChanged;
                }
            }
        }
        
        private static void OnPauseCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AudioPlayer control)
            {
                if (newValue is ICommand newCommand)
                {
                    newCommand.CanExecuteChanged += PauseCommandCanExecuteChanged;
                }
            }
        }

        private static void OnPauseCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AudioPlayer control)
            {
                if (oldValue != null)
                {
                    (oldValue as ICommand).CanExecuteChanged -= PauseCommandCanExecuteChanged;
                }
            }
        }

        private static void PauseCommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (sender is AudioPlayer control)
            {
                control.PauseCommand?.CanExecute(control.PauseCommandParameter);
            }
        }
        
        private static void OnPlayNextCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AudioPlayer control)
            {
                if (newValue is ICommand newCommand)
                {
                    newCommand.CanExecuteChanged += PlayNextCommandCanExecuteChanged;
                }
            }
        }

        private static void OnPlayNextCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AudioPlayer control)
            {
                if (oldValue != null)
                {
                    (oldValue as ICommand).CanExecuteChanged -= PlayNextCommandCanExecuteChanged;
                }
            }
        }

        private static void PlayNextCommandCanExecuteChanged(object sender, EventArgs empty)
        {
            if (sender is AudioPlayer control)
            {
                control.PlayNextCommand?.CanExecute(control.PlayNextCommandParameter);
            }
        }
    }
}
