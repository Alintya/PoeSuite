using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using System;

namespace PoeSuite.Views.Converters
{
    internal class BooleanToColorConverter : IValueConverter
    {
        private readonly SolidColorBrush _green = new SolidColorBrush(Colors.Green);
        private readonly SolidColorBrush _black = new SolidColorBrush(Colors.Green);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool state && state is true
                ? _green
                : _black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
