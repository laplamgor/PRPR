using PRPR.BooruViewer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace PRPR.BooruViewer.Converters
{
    public class TagTypeSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TagType)
            {
                switch ((TagType)value)
                {
                    default:
                    case TagType.None:
                        return Symbol.Placeholder;
                    case TagType.Artist:
                        return Symbol.Edit;
                    case TagType.Character:
                        return Symbol.Contact;
                    case TagType.Copyright:
                        return Symbol.PreviewLink;
                    case TagType.Circle:
                        return Symbol.Home;
                    case TagType.Faults:
                        return Symbol.Important;
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
