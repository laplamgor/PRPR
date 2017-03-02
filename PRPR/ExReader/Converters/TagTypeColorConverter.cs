using PRPR.BooruViewer.Services;
using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace PRPR.ExReader.Converters
{
    public class TagTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ExTagType)
            {
                switch ((ExTagType)value)
                {
                    default:
                    case ExTagType.None:
                        return Color.FromArgb(0xFF, 0x76, 0x76, 0x76);
                    case ExTagType.Artist:
                        return Color.FromArgb(0xFF, 0xCA, 0x50, 0x10);
                    case ExTagType.Character:
                        return Color.FromArgb(0xFF, 0x10, 0x89, 0x3E);
                    case ExTagType.Parody:
                        return Color.FromArgb(0xFF, 0xC2, 0x39, 0xB3);
                    case ExTagType.Group:
                        return Color.FromArgb(0xFF, 0x2D, 0x7D, 0x9A);
                    case ExTagType.Male:
                        return Color.FromArgb(0xFF, 0x00, 0x78, 0xD7);
                    case ExTagType.Female:
                        return Color.FromArgb(0xFF, 0xEA, 0x00, 0x5E);
                    case ExTagType.Language:
                        return Color.FromArgb(0xFF, 0x56, 0x7C, 0x73);
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
