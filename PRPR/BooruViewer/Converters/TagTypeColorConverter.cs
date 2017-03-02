using PRPR.BooruViewer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace PRPR.BooruViewer.Converters
{
    public class TagTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TagType)
            {
                switch ((TagType)value)
                {
                    default:
                    case TagType.None:
                        return Color.FromArgb(0xFF, 0x76, 0x76, 0x76);
                    case TagType.Artist:
                        return Color.FromArgb(0xFF, 0xCA, 0x50, 0x10);
                    case TagType.Character:
                        return Color.FromArgb(0xFF, 0x10, 0x89, 0x3E);
                    case TagType.Copyright:
                        return Color.FromArgb(0xFF, 0xC2, 0x39, 0xB3);
                    case TagType.Circle:
                        return Color.FromArgb(0xFF, 0x2D, 0x7D, 0x9A);
                    case TagType.Faults:
                        return Color.FromArgb(0xFF, 0xE8, 0x11, 0x23);
                }
            }

            return Color.FromArgb(0xFF, 0x76, 0x76, 0x76);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
