using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.Controls
{
    public class RemoteControlPage : ContentPage, IPlayerController, IPlayerElement
    {
        public static readonly BindableProperty PauseCommandProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PauseCommand),
                typeof(ICommand),
                typeof(IPlayerElement),
                null,
                propertyChanging: OnPauseCommandChanging,
                propertyChanged: OnPauseCommandChanged);

        public ICommand PauseCommand
        {
            get => (ICommand)GetValue(PauseCommandProperty);
            set => SetValue(PauseCommandProperty, value);
        }

        public static readonly BindableProperty PauseCommandParameterProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PauseCommandParameter),
                typeof(object),
                typeof(IPlayerElement),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PauseCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public object PauseCommandParameter
        {
            get => GetValue(PauseCommandParameterProperty);
            set => SetValue(PauseCommandParameterProperty, value);
        }

        public static readonly BindableProperty PlayCommandProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PlayCommand),
                typeof(ICommand),
                typeof(IPlayerElement),
                null,
                propertyChanging: OnPlayCommandChanging,
                propertyChanged: OnPlayCommandChanged);

        public ICommand PlayCommand
        {
            get => (ICommand)GetValue(PlayCommandProperty);
            set => SetValue(PlayCommandProperty, value);
        }

        public static readonly BindableProperty PlayCommandParameterProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PlayCommandParameter),
                typeof(object),
                typeof(IPlayerElement),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PlayCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public object PlayCommandParameter
        {
            get => GetValue(PlayCommandParameterProperty);
            set => SetValue(PlayCommandParameterProperty, value);
        }

        public static readonly BindableProperty IsPlayPreviousEnabledProperty
            = BindableProperty.Create(nameof(IsPlayPreviousEnabled),
                typeof(bool),
                typeof(IPlayerElement),
                default);

        public bool IsPlayPreviousEnabled
        {
            get => (bool)GetValue(IsPlayPreviousEnabledProperty);
            set => SetValue(IsPlayPreviousEnabledProperty, value);
        }

        public static readonly BindableProperty PlayPreviousCommandProperty
           = BindableProperty.Create(
               nameof(IPlayerElement.PlayPreviousCommand),
               typeof(ICommand),
               typeof(IPlayerElement),
               null,
               propertyChanging: OnPlayPreviousCommandChanging,
               propertyChanged: OnPlayPreviousCommandChanged);

        public ICommand PlayPreviousCommand {
            get => (ICommand)GetValue(PlayPreviousCommandProperty);
            set => SetValue(PlayPreviousCommandProperty, value);
        }

        public static readonly BindableProperty PlayPreviousCommandParameterProperty
            = BindableProperty.Create(
                nameof(IPlayerElement.PlayPreviousCommandParameter),
                typeof(object),
                typeof(IPlayerElement),
                null,
                propertyChanged: (bindable, oldvalue, newvalue) => PlayPreviousCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public object PlayPreviousCommandParameter
        {
            get => GetValue(PlayPreviousCommandParameterProperty);
            set => SetValue(PlayPreviousCommandParameterProperty, value);
        }

        public static readonly BindableProperty IsPlayNextEnabledProperty
            = BindableProperty.Create(nameof(IsPlayNextEnabled),
                typeof(bool),
                typeof(IPlayerElement),
                default);

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

        public object PlayNextCommandParameter
        {
            get => GetValue(PlayNextCommandParameterProperty);
            set => SetValue(PlayNextCommandParameterProperty, value);
        }

        public ICommand PlayNextCommand
        {
            get => (ICommand)GetValue(PlayNextCommandProperty);
            set => SetValue(PlayNextCommandProperty, value);
        }


        public bool IsPlayNextEnabled
        {
            get => (bool)GetValue(IsPlayNextEnabledProperty);
            set => SetValue(IsPlayNextEnabledProperty, value);
        }

        public static readonly BindableProperty AudioPlayerStateProperty
            = BindableProperty.Create(
                nameof(AudioPlayerState),
                typeof(AudioPlayerState),
                typeof(IPlayerElement),
                AudioPlayerState.Closed);

        public AudioPlayerState AudioPlayerState
        {
            get => (AudioPlayerState)GetValue(AudioPlayerStateProperty);
            set => SetValue(AudioPlayerStateProperty, value);
        }

        public object SelectTrackCommandParameter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICommand SelectTrackCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public double Progress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        public void SendPlayPreviousClicked()
        {
            PlayPreviousCommand?.Execute(PlayPreviousCommandParameter);
        }

        public void SendSelectTrackClicked()
        {
            throw new System.NotImplementedException();
        }

        public void OnSelectTrackCommandCanExecuteChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnPlayCommandCanExecuteChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnPauseCommandCanExecuteChanged(object sender, EventArgs e)
        {
            PauseCommandCanExecuteChanged(this, EventArgs.Empty);
        }

        public void OnPlayNextCommandCanExecuteChanged(object sender, EventArgs e)
        {
            PlayNextCommandCanExecuteChanged(this, EventArgs.Empty);
        }

        public void OnPlayPreviousCommandCanExecuteChanged(object sender, EventArgs e)
        {
            PlayPreviousCommandCanExecuteChanged(this, EventArgs.Empty);
        }

        private static void OnPlayCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (oldValue != null)
            {
                (oldValue as ICommand).CanExecuteChanged -= player.OnPlayCommandCanExecuteChanged;
            }
        }

        private static void OnPlayCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (newValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += player.OnPlayCommandCanExecuteChanged;
            }
            if (player.PlayCommand != null)
            {
                PlayCommandCanExecuteChanged(player, EventArgs.Empty);
            }
        }

        private static void PlayCommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (sender is IPlayerElement element)
            {
                element.PlayCommand?.CanExecute(element.PlayCommandParameter);
            }
        }

        private static void OnPauseCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (newValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += player.OnPauseCommandCanExecuteChanged;
            }
            if (player.PauseCommand != null)
            {
                PauseCommandCanExecuteChanged(player, EventArgs.Empty);
            }
        }

        private static void OnPauseCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (oldValue != null)
            {
                (oldValue as ICommand).CanExecuteChanged -= player.OnPauseCommandCanExecuteChanged;
            }
        }

        private static void PauseCommandCanExecuteChanged(object sender, EventArgs e)
        {
            if (sender is IPlayerElement element)
            {
                element.PauseCommand?.CanExecute(element.PauseCommandParameter);
            }
        }
        
        private static void OnPlayPreviousCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (oldValue != null)
            {
                (oldValue as ICommand).CanExecuteChanged -= player.OnPlayPreviousCommandCanExecuteChanged;
            }
        }

        private static void OnPlayPreviousCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (newValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += player.OnPlayPreviousCommandCanExecuteChanged;
            }
            if (player.PlayPreviousCommand != null)
            {
                PlayPreviousCommandCanExecuteChanged(player, EventArgs.Empty);
            }
        }

        private static void PlayPreviousCommandCanExecuteChanged(object sender, EventArgs empty)
        {
            if (sender is IPlayerElement element)
            {
                if (element.PlayPreviousCommand != null)
                {
                    element.IsPlayPreviousEnabled = element.PlayPreviousCommand.CanExecute(element.PlayNextCommandParameter);
                }
            }
        }

        private static void OnPlayNextCommandChanging(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (oldValue != null)
            {
                (oldValue as ICommand).CanExecuteChanged -= player.OnPlayNextCommandCanExecuteChanged;
            }
        }

        private static void OnPlayNextCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            IPlayerElement player = (IPlayerElement)bindable;
            if (newValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += player.OnPlayNextCommandCanExecuteChanged;
            }
            if (player.PlayNextCommand != null)
            {
                PlayNextCommandCanExecuteChanged(player, EventArgs.Empty);
            }
        }

        private static void PlayNextCommandCanExecuteChanged(object sender, EventArgs empty)
        {
            if (sender is IPlayerElement player)
            {
                if (player.PlayNextCommand != null)
                {
                    player.IsPlayNextEnabled = player.PlayNextCommand.CanExecute(player.PlayNextCommandParameter);
                }
            }
        }
    }
}
