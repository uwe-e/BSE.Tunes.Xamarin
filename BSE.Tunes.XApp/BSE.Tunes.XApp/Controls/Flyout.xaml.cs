using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSE.Tunes.XApp.Controls
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Flyout : ContentView
    {
        private const uint AnimationSpeed = 300;

        private Frame _flyout;
        private BoxView _fader;
        private double _pageHeight;
        private double _flyoutHeight;

        public static readonly BindableProperty FlyoutBackgroundColorProperty =
            BindableProperty.Create(nameof(FlyoutBackgroundColor), typeof(Color), typeof(Flyout), default(Color));

        public Color FlyoutBackgroundColor
        {
            get { return (Color)GetValue(FlyoutBackgroundColorProperty); }
            set { SetValue(FlyoutBackgroundColorProperty, value); }
        }

        public static readonly BindableProperty IsOpenProperty
            = BindableProperty.Create(
                nameof(IsOpen),
                typeof(bool),
                typeof(Flyout),
                false, BindingMode.TwoWay,
                null,
                propertyChanged: OnIsOpenChanged);

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public Flyout()
        {
            InitializeComponent();
            IsVisible = IsOpen;
        }

        private static void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var flyout = (Flyout)bindable;
            if (newValue is bool isOpen)
            {
                if (isOpen)
                {
                    flyout.Open();
                }
                else
                {
                    flyout.Close();
                }
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            _pageHeight = height;
            
            if (_flyout != null)
            {
                _flyout.TranslationY = _pageHeight;
            }
            
            base.OnSizeAllocated(width, height);
        }

        private async void Close()
        {
            await _fader.FadeTo(0, AnimationSpeed, Easing.SinInOut);
            _fader.IsVisible = false;
            await _flyout.TranslateTo(0, _pageHeight, AnimationSpeed, Easing.SinInOut);
            IsVisible = false;
         }

        private async void Open()
        {
            IsVisible = _fader.IsVisible = true;
            await _fader.FadeTo(1, AnimationSpeed, Easing.SinInOut);

            /*
             * for measuring the height of the flyout, this flyout is stored in the 2nd row of the grid with the height "Auto".
             * If we measured the height, we move the flyout into the same row as the fader is stored within.
             * So we can use the flyout as a overlay over the fader.
             */
            if (Grid.GetRow(_flyout) == 1)
            {
                _flyoutHeight = _flyout.Height;
                Grid.SetRow(_flyout, 0);
            }

            await _flyout.TranslateTo(0, _pageHeight - _flyoutHeight, AnimationSpeed, Easing.SinInOut);
        }

        private void OnFaderTapped(object sender, EventArgs e)
        {
            IsOpen = false;
            Close();
        }

        protected override void OnApplyTemplate()
        {
            _flyout = base.GetTemplateChild("PART_Flyout") as Frame;
            _fader = base.GetTemplateChild("PART_Fader") as BoxView;

            base.OnApplyTemplate();
        }
    }
}