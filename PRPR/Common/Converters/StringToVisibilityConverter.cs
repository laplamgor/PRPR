using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PRPR.Common.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        private object GetVisibility(object value)
        {
            if (value == null || !(value is string))
            {
                return Visibility.Collapsed;
            }

            string s = (string)value;
            if (!String.IsNullOrWhiteSpace(s))
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
}
