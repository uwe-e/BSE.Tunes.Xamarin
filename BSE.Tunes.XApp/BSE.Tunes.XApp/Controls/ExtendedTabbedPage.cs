using Xamarin.Forms;

namespace BSE.Tunes.XApp.Controls
{
    public class ExtendedTabbedPage : TabbedPage
    {
        public static readonly BindableProperty AudioPlayerBarProperty
            = BindableProperty.Create(nameof(AudioPlayerBar),
                                      typeof(AudioPlayer),
                                      typeof(ExtendedTabbedPage),
                                      null,
                                      propertyChanged: OnAudioPlayerBarChanged);

        public AudioPlayer AudioPlayerBar
        {
            get { return (AudioPlayer)GetValue(AudioPlayerBarProperty); }
            set { SetValue(AudioPlayerBarProperty, value); }
        }
        
        private static void OnAudioPlayerBarChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var newElement = (Element)newValue;
            if (newElement != null)
            {
                BindableObject.SetInheritedBindingContext(newElement, bindable.BindingContext);
            }
        }
    }
}
