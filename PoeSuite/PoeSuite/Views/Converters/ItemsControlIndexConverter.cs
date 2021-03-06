﻿using System.Windows.Controls;
using System.Windows.Data;
using System;

namespace PoeSuite.Views.Converters
{
    internal class ItemsControlIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ItemCollection itemCollection = (ItemCollection)values[0];
            return (itemCollection.IndexOf(values[1]) + 1).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}