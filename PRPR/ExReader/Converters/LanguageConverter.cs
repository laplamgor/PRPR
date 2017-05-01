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
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is LanguageType languageType)
            {
                switch (languageType)
                {
                    default:
                    case LanguageType.Japanese:
                        return "";
                    case LanguageType.English:
                        return "EN";
                    case LanguageType.Chinese:
                        return "ZH";
                    case LanguageType.Dutch:
                        return "BE";
                    case LanguageType.French:
                        return "FR";
                    case LanguageType.German:
                        return "DE";
                    case LanguageType.Hungarian:
                        return "HU";
                    case LanguageType.Italian:
                        return "IT";
                    case LanguageType.Korean:
                        return "KR";
                    case LanguageType.Polish:
                        return "PL";
                    case LanguageType.Portuguese:
                        return "BR";
                    case LanguageType.Russian:
                        return "RU";
                    case LanguageType.Spanish:
                        return "ES";
                    case LanguageType.Thai:
                        return "TH";
                    case LanguageType.Vietnamese:
                        return "VN";
                    case LanguageType.NotApplicable:
                        return "N/A";
                    case LanguageType.Other:
                        return "?";
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
