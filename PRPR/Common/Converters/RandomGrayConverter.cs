using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PRPR.Common.Converters
{

    public class RandomGrayConverter : IValueConverter
    {
        private static Random rnd = new Random();


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte grayLevel = (byte) rnd.Next(32, 128);
            return new SolidColorBrush(Color.FromArgb(255, grayLevel, grayLevel, grayLevel));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
