using PRPR.ExReader.Models.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.Models
{
    public class ExConfig : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        private List<int> ExcludedCodes
        {
            get
            {
                var splitedCodes = ExSettings.Current.xl.Split('x');
                if (splitedCodes.Length == 1 && splitedCodes[0] == "")
                {
                    return new List<int>();
                }
                else
                {
                    var list = new List<int>();
                    foreach (var item in splitedCodes)
                    {
                        list.Add(int.Parse(item));
                    }
                    return list;
                }
            }

            set
            {
                ExSettings.Current.xl = string.Join("x", value);
            }
        }
        

        private void SetExcludedLanuage(LanguageType language, CreationType creation, bool isExcluded)
        {
            var code = (int)language + (int)creation;
            if (isExcluded)
            {
                var temp = ExcludedCodes;
                if (!temp.Contains(code))
                {
                    temp.Add(code);
                    ExcludedCodes = temp;
                }
            }
            else
            {
                var temp = ExcludedCodes;
                if (temp.Contains(code))
                {
                    temp.Remove(code); ;
                    ExcludedCodes = temp;
                }
            }

            NotifyPropertyChanged($"Excluded{language.ToString()}{creation.ToString()}");
            NotifyPropertyChanged($"Excluded{language.ToString()}");
        }

        private bool CheckExcludedLanuage(LanguageType language, CreationType creation)
        {
            var code = (int)language + (int)creation;
            return ExcludedCodes.Contains(code);
        }



        public bool? ExcludedJapanese
        {
            get
            {
                if (ExcludedJapaneseTranslated == ExcludedJapaneseRewrite)
                {
                    return ExcludedJapaneseTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedJapaneseTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Japanese, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Japanese, CreationType.Translated, value);
            }
        }

        public bool ExcludedJapaneseRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Japanese, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Japanese, CreationType.Rewrite, value);
            }
        }



        public bool? ExcludedEnglish
        {
            get
            {
                if (ExcludedEnglishOriginal == ExcludedEnglishTranslated && ExcludedEnglishTranslated == ExcludedEnglishRewrite)
                {
                    return ExcludedEnglishTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedEnglishOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.English, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.English, CreationType.Translated, value);
            }
        }

        public bool ExcludedEnglishTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.English, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.English, CreationType.Translated, value);
            }
        }

        public bool ExcludedEnglishRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.English, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.English, CreationType.Rewrite, value);
            }
        }




        public bool? ExcludedChinese
        {
            get
            {
                if (ExcludedChineseOriginal == ExcludedChineseTranslated && ExcludedChineseTranslated == ExcludedChineseRewrite)
                {
                    return ExcludedChineseTranslated;
                }
                else
                {
                    return null;
                }
            }
        }


        public bool ExcludedChineseOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Chinese, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Chinese, CreationType.Original, value);
            }
        }

        public bool ExcludedChineseTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Chinese, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Chinese, CreationType.Translated, value);
            }
        }

        public bool ExcludedChineseRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Chinese, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Chinese, CreationType.Rewrite, value);
            }
        }


        

        public bool? ExcludedDutch
        {
            get
            {
                if (ExcludedDutchOriginal == ExcludedDutchTranslated && ExcludedDutchTranslated == ExcludedDutchRewrite)
                {
                    return ExcludedDutchTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedDutchOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Dutch, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Dutch, CreationType.Original, value);
            }
        }

        public bool ExcludedDutchTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Dutch, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Dutch, CreationType.Translated, value);
            }
        }

        public bool ExcludedDutchRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Dutch, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Dutch, CreationType.Rewrite, value);
            }
        }



        public bool? ExcludedFrench
        {
            get
            {
                if (ExcludedFrenchOriginal == ExcludedFrenchTranslated && ExcludedFrenchTranslated == ExcludedFrenchRewrite)
                {
                    return ExcludedFrenchTranslated;
                }
                else
                {
                    return null;
                }
            }
        }


        public bool ExcludedFrenchOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.French, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.French, CreationType.Original, value);
            }
        }

        public bool ExcludedFrenchTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.French, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.French, CreationType.Translated, value);
            }
        }

        public bool ExcludedFrenchRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.French, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.French, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedGerman
        {
            get
            {
                if (ExcludedGermanOriginal == ExcludedGermanTranslated && ExcludedGermanTranslated == ExcludedGermanRewrite)
                {
                    return ExcludedGermanTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedGermanOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.German, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.German, CreationType.Original, value);
            }
        }

        public bool ExcludedGermanTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.German, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.German, CreationType.Translated, value);
            }
        }

        public bool ExcludedGermanRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.German, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.German, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedHungarian
        {
            get
            {
                if (ExcludedHungarianOriginal == ExcludedHungarianTranslated && ExcludedHungarianTranslated == ExcludedHungarianRewrite)
                {
                    return ExcludedHungarianTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedHungarianOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Hungarian, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Hungarian, CreationType.Original, value);
            }
        }

        public bool ExcludedHungarianTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Hungarian, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Hungarian, CreationType.Translated, value);
            }
        }

        public bool ExcludedHungarianRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Hungarian, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Hungarian, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedItalian
        {
            get
            {
                if (ExcludedItalianOriginal == ExcludedItalianTranslated && ExcludedItalianTranslated == ExcludedItalianRewrite)
                {
                    return ExcludedItalianTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedItalianOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Italian, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Italian, CreationType.Original, value);
            }
        }

        public bool ExcludedItalianTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Italian, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Italian, CreationType.Translated, value);
            }
        }

        public bool ExcludedItalianRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Italian, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Italian, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedKorean
        {
            get
            {
                if (ExcludedKoreanOriginal == ExcludedKoreanTranslated && ExcludedKoreanTranslated == ExcludedKoreanRewrite)
                {
                    return ExcludedKoreanTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedKoreanOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Korean, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Korean, CreationType.Original, value);
            }
        }

        public bool ExcludedKoreanTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Korean, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Korean, CreationType.Translated, value);
            }
        }

        public bool ExcludedKoreanRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Korean, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Korean, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedPolish
        {
            get
            {
                if (ExcludedPolishOriginal == ExcludedPolishTranslated && ExcludedPolishTranslated == ExcludedPolishRewrite)
                {
                    return ExcludedPolishTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedPolishOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Polish, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Polish, CreationType.Original, value);
            }
        }

        public bool ExcludedPolishTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Polish, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Polish, CreationType.Translated, value);
            }
        }

        public bool ExcludedPolishRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Polish, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Polish, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedPortuguese
        {
            get
            {
                if (ExcludedPortugueseOriginal == ExcludedPortugueseTranslated && ExcludedPortugueseTranslated == ExcludedPortugueseRewrite)
                {
                    return ExcludedPortugueseTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedPortugueseOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Portuguese, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Portuguese, CreationType.Original, value);
            }
        }

        public bool ExcludedPortugueseTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Portuguese, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Portuguese, CreationType.Translated, value);
            }
        }

        public bool ExcludedPortugueseRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Portuguese, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Portuguese, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedRussian
        {
            get
            {
                if (ExcludedRussianOriginal == ExcludedRussianTranslated && ExcludedRussianTranslated == ExcludedRussianRewrite)
                {
                    return ExcludedRussianTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedRussianOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Russian, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Russian, CreationType.Original, value);
            }
        }

        public bool ExcludedRussianTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Russian, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Russian, CreationType.Translated, value);
            }
        }

        public bool ExcludedRussianRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Russian, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Russian, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedSpanish
        {
            get
            {
                if (ExcludedSpanishOriginal == ExcludedSpanishTranslated && ExcludedSpanishTranslated == ExcludedSpanishRewrite)
                {
                    return ExcludedSpanishTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedSpanishOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Spanish, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Spanish, CreationType.Original, value);
            }
        }

        public bool ExcludedSpanishTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Spanish, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Spanish, CreationType.Translated, value);
            }
        }

        public bool ExcludedSpanishRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Spanish, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Spanish, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedThai
        {
            get
            {
                if (ExcludedThaiOriginal == ExcludedThaiTranslated && ExcludedThaiTranslated == ExcludedThaiRewrite)
                {
                    return ExcludedThaiTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedThaiOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Thai, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Thai, CreationType.Original, value);
            }
        }

        public bool ExcludedThaiTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Thai, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Thai, CreationType.Translated, value);
            }
        }

        public bool ExcludedThaiRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Thai, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Thai, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedVietnamese
        {
            get
            {
                if (ExcludedVietnameseOriginal == ExcludedVietnameseTranslated && ExcludedVietnameseTranslated == ExcludedVietnameseRewrite)
                {
                    return ExcludedVietnameseTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedVietnameseOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Vietnamese, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Vietnamese, CreationType.Original, value);
            }
        }

        public bool ExcludedVietnameseTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Vietnamese, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Vietnamese, CreationType.Translated, value);
            }
        }

        public bool ExcludedVietnameseRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Vietnamese, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Vietnamese, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedNotApplicable
        {
            get
            {
                if (ExcludedNotApplicableOriginal == ExcludedNotApplicableTranslated && ExcludedNotApplicableTranslated == ExcludedNotApplicableRewrite)
                {
                    return ExcludedNotApplicableTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedNotApplicableOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.NotApplicable, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.NotApplicable, CreationType.Original, value);
            }
        }

        public bool ExcludedNotApplicableTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.NotApplicable, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.NotApplicable, CreationType.Translated, value);
            }
        }

        public bool ExcludedNotApplicableRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.NotApplicable, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.NotApplicable, CreationType.Rewrite, value);
            }
        }


        public bool? ExcludedOther
        {
            get
            {
                if (ExcludedOtherOriginal == ExcludedOtherTranslated && ExcludedOtherTranslated == ExcludedOtherRewrite)
                {
                    return ExcludedOtherTranslated;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool ExcludedOtherOriginal
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Other, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Other, CreationType.Original, value);
            }
        }

        public bool ExcludedOtherTranslated
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Other, CreationType.Translated);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Other, CreationType.Translated, value);
            }
        }

        public bool ExcludedOtherRewrite
        {
            get
            {
                return CheckExcludedLanuage(LanguageType.Other, CreationType.Rewrite);
            }
            set
            {
                SetExcludedLanuage(LanguageType.Other, CreationType.Rewrite, value);
            }
        }

    }

    enum LanguageType
    {
        Japanese = 0,
        English = 1,
        Chinese = 10,
        Dutch = 20,
        French = 30,
        German = 40,
        Hungarian = 50,
        Italian = 60,
        Korean = 70,
        Polish = 80,
        Portuguese = 90,
        Russian = 100,
        Spanish = 110,
        Thai = 120,
        Vietnamese = 130,
        NotApplicable = 254,
        Other = 255
    }

    enum CreationType
    {
        Original = 0,
        Translated = 1024,
        Rewrite = 2048
    }
}
