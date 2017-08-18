using System;
using System.Globalization;
using Xamarin.Forms;

namespace BLEDemo
{
    public class GuidToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var theValue = (Guid)value;
            return theValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid guid;
            Guid.TryParse(value as string, out guid);
            return guid;
        }
    }
}