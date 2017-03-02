using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace PRPR.ExReader.Converters
{
    public class TagTypeSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ExTagType)
            {
                switch ((ExTagType)value)
                {
                    default:
                    case ExTagType.None:
                        return Symbol.Placeholder;
                    case ExTagType.Artist:
                        return Symbol.Edit;
                    case ExTagType.Character:
                        return Symbol.Contact;
                    case ExTagType.Parody:
                        return Symbol.PreviewLink;
                    case ExTagType.Male:
                        return Symbol.Contact2;
                    case ExTagType.Female:
                        return Symbol.Contact2;
                    case ExTagType.Language:
                        return Symbol.Character;
                    case ExTagType.Group:
                        return Symbol.Home;
                }
            }

            return Symbol.Placeholder;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
