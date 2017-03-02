using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PRPR.Common.Converters
{
    public class FileSizeConverter : IValueConverter
    {
        static double RoundToSignificantDigits(double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ulong byteCount = 0;
            if (ulong.TryParse(value.ToString(), out byteCount))
            {

                if (byteCount < 1024) // <1KB
                {
                    return $"{byteCount} Bytes";
                }
                else if (byteCount < 1048576) // <1MB
                {
                    var rounded = RoundToSignificantDigits(byteCount / 1024.0, 3);
                    return $"{rounded} KB";
                }
                else if (byteCount < 1073741824) // <1GB
                {
                    var rounded = RoundToSignificantDigits(byteCount / 1048576.0, 3);
                    return $"{rounded} MB";
                }
                else if (byteCount < 1099511627776) // <1TB
                {
                    var rounded = RoundToSignificantDigits(byteCount / 1073741824.0, 3);
                    return $"{rounded} GB";
                }
                else
                {
                    var rounded = RoundToSignificantDigits(byteCount / 1099511627776, 3);
                    return $"{rounded} TB";
                }
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
