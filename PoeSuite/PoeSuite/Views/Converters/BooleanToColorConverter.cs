using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PoeSuite.Views.Converters
{
    class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // szuka bljad why create a new color every time lol xd rofl, fix
            return value is bool state && state is true
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
