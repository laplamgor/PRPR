using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PRPR.Common.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private object GetVisibility(object value)
        {
            if (value == null || !(value is bool))
            {
                return Visibility.Visible;
            }

            bool b = (bool)value;
            if (b)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class BooleanToInvisibilityConverter : IValueConverter
    {
        private object GetVisibility(object value)
        {
            if (value == null || !(value is bool))
            {
                return Visibility.Collapsed;
            }

            bool b = (bool)value;
            if (b)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
