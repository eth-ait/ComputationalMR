using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Windows.Data;
using ConstraintUI.Model;

namespace ConstraintUI.Converter
{
    public class ListAndIndexToElementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3)
                return null;

            var list = values[0] as ObservableCollection<ElementModel>;
            var index = (int)values[1];

            if ( list == null || list.Count < 1)
                return null;
            
            if (list.Count > index)
                return list[index];

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}