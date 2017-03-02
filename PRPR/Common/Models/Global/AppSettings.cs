using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



    }
}
