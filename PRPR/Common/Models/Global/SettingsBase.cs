using PRPR.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;

namespace PRPR.Common.Models.Global
{
    public class SettingsBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // NotifyPropertyChanged will raise the PropertyChanged event, 
        // passing the source property that is being updated.
        public async void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    });

                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Debug.WriteLine(ex.Message);
                        ex = ex.InnerException;
                    }
                }
            }
        }
        #endregion


        #region Get / Set helpers
        protected static string GetCallerName([CallerMemberName] string name = null)
        {
            return name;
        }

        protected void AddOrUpdateValue(string Key, Object value, bool isRoaming)
        {
            var settings = isRoaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            
            // Store the new value
            settings.Values[Key] = value;
            NotifyPropertyChanged(Key);
        }

        protected T GetValueOrDefault<T>(string Key, T defaultValue, bool isRoaming)
        {
            T value;
            var settings = isRoaming ? RoamingSettings : LocalSettings;

            try
            {
                // If the key exists, retrieve the value.
                if (settings.Values.ContainsKey(Key))
                {
                    value = (T)settings.Values[Key];
                }
                // Otherwise, use the default value.
                else
                {
                    value = defaultValue;
                }
                return value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return defaultValue;
            }

        }
        #endregion


        protected ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        protected ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;
    }
}
