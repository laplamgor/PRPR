using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PRPR.ExReader.Converters
{
    public class ShortTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                var s = (value as string);
                return GetShortTitle(s);
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

        public static string GetShortTitle(string fullTitle)
        {
            return Regex.Replace(fullTitle, @"(\([^\(\)]*\)|\[[^\[\]]*\])", m => "").Trim();
        }
    }
}
