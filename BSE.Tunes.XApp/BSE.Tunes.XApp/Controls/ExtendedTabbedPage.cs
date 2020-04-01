using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.Controls
{
    public class ExtendedTabbedPage : TabbedPage, IPlayerController
    {
        public static readonly BindableProperty PlayCommandProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PlayCommand),
                typeof(ICommand),
                typeof(IPlayerElement),
                null,
                propertyChanging: OnPlayCommandChanging,
                propertyChanged: OnPlayCommandChanged);

        public static readonly BindableProperty PlayCommandParameterProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PlayCommandParameter),
                typeof(object),
                typeof(IPlayerElement),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PlayCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public static readonly BindableProperty PauseCommandProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PauseCommand),
                typeof(ICommand),
                typeof(IPlayerElement),
                null,
                propertyChanging: OnPauseCommandChanging,
                propertyChanged: OnPauseCommandChanged);

        public static readonly BindableProperty PauseCommandParameterProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PauseCommandParameter),
                typeof(object),
                typeof(IPlayerElement),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PauseCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public static readonly BindableProperty PlayNextCommandProperty
           = BindableProperty.Create(
               nameof(IPlayerElement.PlayNextCommand),
               typeof(ICommand),
               typeof(IPlayerElement),
               null,
               propertyChanging: OnPlayNextCommandChanging,
               propertyChanged: OnPlayNextCommandChanged);



        public static readonly BindableProperty PlayNextCommandParameterProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PlayNextCommandParameter),
                typeof(object),
                typeof(IPlayerElement),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PlayNextCommandCanExecuteChanged(bindable, EventArgs.Empty));



        public ICommand PlayCommand
        {
            get { return (ICommand)GetValue(PlayCommandProperty); }
            set { SetValue(PlayCommandProperty, value); }
        }

        public object PlayCommandParameter
        {
            get { return GetValue(PlayCommandParameterProperty); }
            set { SetValue(PlayCommandParameterProperty, value); }
        }

        public ICommand PauseCommand
        {
            get { return (ICommand)GetValue(PauseCommandProperty); }
            set { SetValue(PauseCommandProperty, value); }
        }

        public object PauseCommandParameter
        {
            get { return GetValue(PauseCommandParameterProperty); }
            set { SetValue(PauseCommandParameterProperty, value); }
        }

        public ICommand PlayNextCommand
        {
            get { return (ICommand)GetValue(PlayNextCommandProperty); }
            set { SetValue(PlayNextCommandProperty, value); }
        }

        public object PlayNextCommandParameter
        {
            get { return GetValue(PlayNextCommandParameterProperty); }
            set { SetValue(PlayNextCommandParameterProperty, value); }
        }

        public EventHandler<PlayStateChangedEventArgs> PlayStateChanged;

        PlayState CurrentPlayState { get; set; } = PlayState.Stopped;

        public void SendPauseClicked()
        {
            PlayStateChanged?.Invoke(this, new PlayStateChangedEventArgs(PlayState.Playing, PlayState.Paused));
            PauseCommand?.Execute(PauseCommandParameter);
        }

        public void SendPlayClicked()
        {
            PlayStateChanged?.Invoke(this, new PlayStateChangedEventArgs(CurrentPlayState, PlayState.Playing));
            PlayCommand?.Execute(PlayCommandParameter);
        }

        private static void OnPlayCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedTabbedPage control)
            {
                if (oldValue != null)
                {
                    (oldValue as ICommand).CanExecuteChanged -= PlayCommandCanExecuteChanged;
                }
            }
        }

        private static void OnPlayCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedTabbedPage control)
            {
                if (newValue is ICommand newCommand)
                {
                    newCommand.CanExecuteChanged += PlayCommandCanExecuteChanged;
                }
            }
        }

        private static void PlayCommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (sender is ExtendedTabbedPage control)
            {
                control.PlayCommand?.CanExecute(control.PlayCommandParameter);
            }
        }
        private static void OnPauseCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedTabbedPage control)
            {
                if (oldValue != null)
                {
                    (oldValue as ICommand).CanExecuteChanged -= PauseCommandCanExecuteChanged;
                }
            }
        }

        private static void OnPauseCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedTabbedPage control)
            {
                if (newValue is ICommand newCommand)
                {
                    newCommand.CanExecuteChanged += PauseCommandCanExecuteChanged;
                }
            }
        }

        private static void PauseCommandCanExecuteChanged(object sender, EventArgs empty)
        {
            if (sender is ExtendedTabbedPage control)
            {
                control.PauseCommand?.CanExecute(control.PauseCommandParameter);
            }
        }

        public void SendPlayNextClicked()
        {
            //PlayStateChanged?.Invoke(this, new PlayStateChangedEventArgs(CurrentPlayState, PlayState.Playing));
            PlayNextCommand?.Execute(PlayNextCommandParameter);
        }
        private static void OnPlayNextCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedTabbedPage control)
            {
                if (oldValue != null)
                {
                    (oldValue as ICommand).CanExecuteChanged -= PlayNextCommandCanExecuteChanged;
                }
            }
        }

        private static void OnPlayNextCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedTabbedPage control)
            {
                if (newValue is ICommand newCommand)
                {
                    newCommand.CanExecuteChanged += PlayNextCommandCanExecuteChanged;
                }
            }
        }

        private static void PlayNextCommandCanExecuteChanged(object sender, EventArgs empty)
        {
            if (sender is ExtendedTabbedPage control)
            {
                control.PlayNextCommand?.CanExecute(control.PlayNextCommandParameter);
            }
        }
    }
}
