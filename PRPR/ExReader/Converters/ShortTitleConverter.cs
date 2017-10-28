using PRPR.ExReader.Models;
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
            if (value is ExGallery)
            {
                var g = (value as ExGallery);
                string t;
                if (!String.IsNullOrEmpty(g.JapaneseTitle))
                {
                    t = g.JapaneseTitle;
                }
                else
                {
                    // Fallback to use english title
                    t = g.Title;
                }

                return GetPrefix(g) + GetShortTitle(t);
            }
            else
            {
                return null;
            }
        }

        private static string GetPrefix(ExGallery g)
        {
            switch (g.ParsedLanguage)
            {
                default:
                case LanguageType.Other:
                case LanguageType.NotApplicable:
                case LanguageType.Japanese:
                    return "";
                case LanguageType.English:
                    return "[Eng]";
                case LanguageType.Chinese:
                    return "[中]";
                case LanguageType.Dutch:
                    return "[Dutch]";
                case LanguageType.French:
                    return "[Fr]";
                case LanguageType.German:
                    return "[De]";
                case LanguageType.Hungarian:
                    return "[Magyar]";
                case LanguageType.Italian:
                    return "[it]";
                case LanguageType.Korean:
                    return "[한]";
                case LanguageType.Polish:
                    return "[Pol]";
                case LanguageType.Portuguese:
                    return "[Port]";
                case LanguageType.Russian:
                    return "[русский]";
                case LanguageType.Spanish:
                    return "[Es]";
                case LanguageType.Thai:
                    return "[ไทย]";
                case LanguageType.Vietnamese:
                    return "[Việt]";
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
