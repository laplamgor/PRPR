using PRPR.Common.Models.Global;
using PRPR.Common.Services;
using PRPR.ExReader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;

namespace PRPR.ExReader.Models.Global
{
    public class ExSettings : SettingsBase
    {
        public static ExSettings Current
        {
            get
            {
                return Application.Current.Resources["ExSettings"] as ExSettings;
            }
        }


        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;




        private ExSearchConfig _searchConfig = null;

        public ExSearchConfig SearchConfig
        {
            get
            {
                if (_searchConfig == null)
                {
                    var s = GetValueOrDefault<string>(GetCallerName(), null, false);
                    if (s != null)
                    {
                        _searchConfig = SerializationService.DeserializeFromString<ExSearchConfig>(s);
                    }
                    else
                    {
                        _searchConfig = new ExSearchConfig();
                    }

                    _searchConfig.PropertyChanged += SearchConfig_PropertyChanged;
                }

                return _searchConfig;
            }
            set
            {
                if (_searchConfig != null)
                {
                    // Old handler
                    _searchConfig.PropertyChanged -= SearchConfig_PropertyChanged;
                }

                if (value != null)
                {
                    // Add new handler
                    value.PropertyChanged += SearchConfig_PropertyChanged;
                }
                else
                {
                    // WTF
                }



                var s = SerializationService.SerializeToString(value);
                AddOrUpdateValue(GetCallerName(), s, false);
            }
        }

        private void SearchConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var s = SerializationService.SerializeToString(_searchConfig);
            AddOrUpdateValue(nameof(SearchConfig), s, false);
        }



        private ExGalleryFilter _galleryFilter = null;

        public ExGalleryFilter GalleryFilter
        {
            get
            {
                if (_galleryFilter == null)
                {
                    var s = GetValueOrDefault<string>(GetCallerName(), null, false);
                    if (s != null)
                    {
                        _galleryFilter = SerializationService.DeserializeFromString<ExGalleryFilter>(s);
                    }
                    else
                    {
                        _galleryFilter = new ExGalleryFilter();
                    }

                    _galleryFilter.PropertyChanged += SearchPostFilter_PropertyChanged;
                }

                return _galleryFilter;
            }
            set
            {
                if (_galleryFilter != null)
                {
                    // Old handler
                    _galleryFilter.PropertyChanged -= SearchPostFilter_PropertyChanged;
                }

                if (value != null)
                {
                    // Add new handler
                    value.PropertyChanged += SearchPostFilter_PropertyChanged;
                }
                else
                {
                    // WTF
                }



                var s = SerializationService.SerializeToString(value);
                AddOrUpdateValue(GetCallerName(), s, false);
            }
        }

        private void SearchPostFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var s = SerializationService.SerializeToString(_galleryFilter);
            AddOrUpdateValue(nameof(GalleryFilter), s, false);
        }









        public string ECookie
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "", false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(AvatarUrl));
            }
        }

        public string AvatarUrl
        {
            get
            {
                try
                {

                    var uid = ExClient.CheckForMemberID(ECookie);
                    return $"https://forums.e-hentai.org/uploads/av-{uid}.jpg";
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }


        public DateTime ECookieExpire
        {
            get
            {
                return DateTime.FromBinary(GetValueOrDefault<long>(GetCallerName(), DateTime.MinValue.ToBinary(), false));
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value.ToBinary(), false);
            }
        }


        public bool IsBlurAvailable
        {
            get
            {
                //return false;
                return ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateBackdropBrush");
            }
        }


        public string xl
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "", false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }


        public List<int> ExcludedLanguages
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



        // Real Local setting
        public bool StartFromRightToLeft
        {
            get
            {
                return GetValueOrDefault<bool>(GetCallerName(), false, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }


    }
}
