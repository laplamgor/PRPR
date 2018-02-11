using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace PRPR.Common.Models.Global
{
    public class AppSettings : SettingsBase
    {
        public static AppSettings Current
        {
            get
            {
                return Application.Current.Resources["AppSettings"] as AppSettings;
            }
        }

        public Size ScreenSize
        {
            get
            {
                return new Size(ScreenWidth, ScreenHeight);
            }
        }

        public double ScreenWidth
        {
            get
            {
                return GetValueOrDefault<double>(GetCallerName(), 0, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(ScreenSize));
            }
        }

        public double ScreenHeight
        {
            get
            {
                return GetValueOrDefault<double>(GetCallerName(), 0, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(ScreenSize));
            }
        }


        public int ThemeSelectedIndex
        {
            get
            {
                return GetValueOrDefault<int>(GetCallerName(), 0, false);
            }
            set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
            }
        }



        #region app version
        public string SavedAppVersion
        {
            get
            {
                return GetValueOrDefault<string>(GetCallerName(), "0.0.0.0", false);
            }
            private set
            {
                AddOrUpdateValue(GetCallerName(), value, false);
                NotifyPropertyChanged(nameof(HasUpdate));
            }
        }

        public bool HasUpdate
        {
            get
            {
                return SavedAppVersion != CurrentAppVersion;
            }
        }

        public string CurrentAppVersion
        {
            get
            {
                var v = Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", v.Major, v.Minor, v.Build, v.Revision);
            }
        }

        public void UpdateNoticed()
        {
            try
            {
                SavedAppVersion = CurrentAppVersion;
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
