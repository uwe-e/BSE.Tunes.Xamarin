using Xamarin.Forms;

namespace BSE.Tunes.XApp.PlatformConfiguration.iOSSpecific
{

    public static class Button
    {
        public static readonly BindableProperty UILineBreakModeProperty =
            BindableProperty.Create(nameof(UILineBreakMode),
                typeof(UILineBreakMode), typeof(Button), UILineBreakMode.Default);

        public static UILineBreakMode GetUILineBreakMode(BindableObject element)
            => (UILineBreakMode)element.GetValue(UILineBreakModeProperty);

        public static void SetUILineBreakMode(BindableObject element, UILineBreakMode value)
            => element.SetValue(UILineBreakModeProperty, value);

        public static readonly BindableProperty UITextAlignmentProperty =
            BindableProperty.Create(nameof(UITextAlignment),
                typeof(UITextAlignment), typeof(Button), UITextAlignment.Left);

        public static UITextAlignment GetUITextAlignment(BindableObject element)
            => (UITextAlignment)element.GetValue(UITextAlignmentProperty);

        public static void SetUITextAlignment(BindableObject element, UITextAlignment value)
            => element.SetValue(UITextAlignmentProperty, value);

    }
}
