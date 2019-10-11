using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ConstraintUI.Util;

namespace ConstraintUI.Converter
{
    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return DependencyProperty.UnsetValue;

            var intValue = (int) value;
            
            var color = ColorUtil.HLStoRGB(202 , (50 + intValue * 5) / 100.0, 100.0 / 100.0);
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}