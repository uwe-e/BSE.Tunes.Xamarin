using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.Converter
{
    public class AudioPlayerStateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AudioPlayerState audioPlayerState = (AudioPlayerState)value;
            if (audioPlayerState == AudioPlayerState.Playing)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
