using System;
using System.Globalization;
using System.Windows.Data;

namespace PoeSuite.Views.Converters
{
    public class IndexMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = (ListCollectionView)values[1];
            return collection.IndexOf(values[0]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("GetIndexMultiConverter_ConvertBack");
        }
    }
}
