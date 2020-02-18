using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace Draw
{
    public class CustomConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ICollection<Canvas> list = new List<Canvas>();
            foreach (var item in values)
                list.Add((Canvas)item);

            return list;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}