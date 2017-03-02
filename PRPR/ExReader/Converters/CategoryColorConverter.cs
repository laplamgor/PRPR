using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace PRPR.ExReader.Converters
{
    public class CategoryColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                switch (value as string)
                {
                    case "Doujinshi":
                        return Color.FromArgb(255, 0xD1, 0x34, 0x38);
                    case "Manga":
                        return Color.FromArgb(255, 0xCA, 0x50, 0x10);
                    case "Artist CG Sets":
                        return Color.FromArgb(255, 0xFF, 0xB9, 0x00);
                    case "Game CG Sets":
                        return Color.FromArgb(255, 0x10, 0x7C, 0x10);
                    case "Western":
                        return Color.FromArgb(255, 0x64, 0x7C, 0x64);
                    case "Non-H":
                        return Color.FromArgb(255, 0x00, 0x78, 0xD7);
                    case "Image Sets":
                        return Color.FromArgb(255, 0x30, 0x17, 0x98);
                    case "Cosplay":
                        return Color.FromArgb(255, 0x88, 0x17, 0x98);
                    case "Asian Porn":
                        return Color.FromArgb(255, 0xE3, 0x00, 0xBC);
                    case "Misc":
                        return Color.FromArgb(255, 0x76, 0x76, 0x76);
                    default:
                        break;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
