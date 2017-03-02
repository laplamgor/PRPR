using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PRPR.ExReader.Converters
{
    public class BoolToFlowDirection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                if ((bool) value)
                {
                    return FlowDirection.RightToLeft;
                }
                else
                {
                    return FlowDirection.LeftToRight;
                }
            }
            else
            {
                // Default value
                return FlowDirection.LeftToRight;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
