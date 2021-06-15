﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSE.Tunes.XApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomFlyoutPage : ContentPage
    {
        private ContentView _flyout;
        private BoxView _fader;
        private TapGestureRecognizer _faderTabGesture;
        private Button _dismissButton;
        private double _pageHeight;
        private double _flyoutHeight;
        
        public static readonly BindableProperty FlyoutBackgroundColorProperty =
            BindableProperty.Create(nameof(FlyoutBackgroundColor), typeof(Color), typeof(BottomFlyoutPage), default(Color));

        public Color FlyoutBackgroundColor
        {
            get => (Color)GetValue(FlyoutBackgroundColorProperty);
            set => SetValue(FlyoutBackgroundColorProperty, value);
        }

        public static readonly BindableProperty TransparentBackgroundColorProperty =
            BindableProperty.Create(nameof(TransparentBackgroundColor), typeof(Color), typeof(BottomFlyoutPage), Color.FromHex("#d8000000"));

        public Color TransparentBackgroundColor
        {
            get => (Color)GetValue(TransparentBackgroundColorProperty);
            set => SetValue(TransparentBackgroundColorProperty, value);
        }

        public static readonly BindableProperty CloseFlyoutCommandProperty =
            BindableProperty.Create(
                nameof(CloseFlyoutCommand), typeof(ICommand),
                typeof(BottomFlyoutPage),
                null);

        public ICommand CloseFlyoutCommand
        {
            get => (ICommand)GetValue(CloseFlyoutCommandProperty);
            set => SetValue(CloseFlyoutCommandProperty, value);
        }

        public static readonly BindableProperty CloseButtonTextProperty =
            BindableProperty.Create(
                nameof(CloseButtonText), typeof(string), typeof(Button), "Close");

        public string CloseButtonText
        {
            get { return (string)GetValue(CloseButtonTextProperty); }
            set { SetValue(CloseButtonTextProperty, value); }
        }

        public BottomFlyoutPage()
        {
            InitializeComponent();
            BackgroundColor = Color.Transparent;

            //On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
        }

        public async System.Threading.Tasks.Task AppearingAnimation()
        {
            await _fader.FadeTo(1, 100, Easing.SinInOut);

            if (Grid.GetRow(_flyout) == 1)
            {
                _flyoutHeight = _flyout.Height;
                Grid.SetRow(_flyout, 0);
            }

            await _flyout.TranslateTo(0, _pageHeight - _flyoutHeight, 300, Easing.SinInOut);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            _pageHeight = height;

            if (_flyout != null)
            {
                _flyout.TranslationY = _pageHeight;
            }

            _fader.HeightRequest = _pageHeight;

            base.OnSizeAllocated(width, height);
        }

        public async System.Threading.Tasks.Task DisappearingAnimation()
        {
            await _flyout?.TranslateTo(0, _pageHeight, 300, Easing.SinInOut);
        }

        protected override void OnApplyTemplate()
        {
            _flyout = base.GetTemplateChild("PART_Flyout") as ContentView;
            _fader = base.GetTemplateChild("PART_Fader") as BoxView;
            _dismissButton = base.GetTemplateChild("PART_DismissButton") as Button;
            

            base.OnApplyTemplate();
        }

        protected override void OnAppearing()
        {
            _faderTabGesture = new TapGestureRecognizer();
            _faderTabGesture.Tapped += OnDismissModal;
            _fader.GestureRecognizers.Add(_faderTabGesture);
            
            _dismissButton.Clicked += OnDismissModal;
            
            base.OnAppearing();
        }


        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    _pageHeight = Height;
        //    if (_flyout != null)
        //    {
        //        _flyout.TranslationY = _pageHeight;
        //    }
        //}


        protected override void OnDisappearing()
        {
            _dismissButton.Clicked -= OnDismissModal;
            _faderTabGesture.Tapped -= OnDismissModal;

            base.OnDisappearing();
        }

        private void OnDismissModal(object sender, EventArgs e)
        {
            CloseFlyoutCommand?.Execute(null);
        }
    }
}