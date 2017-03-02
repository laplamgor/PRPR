using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PRPR.BooruViewer.Converters
{
    public class TagNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                var s = (value as string).Replace('_', ' ');
                
                return "#" + Regex.Replace(s.ToLower(), @"(\b[a-zA-Z])", m => m.Value.ToUpper()).Replace(' ', '_');
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
