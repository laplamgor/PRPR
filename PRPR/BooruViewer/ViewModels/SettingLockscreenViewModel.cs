using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.ViewModels
{
    public class SettingLockscreenViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion



        public PostFilter LockscreenPostFilter
        {
            get
            {
                return YandeSettings.Current.LockscreenPostFilter;
            }

            set
            {
                YandeSettings.Current.LockscreenPostFilter = value;
                NotifyPropertyChanged(nameof(LockscreenPostFilter));
            }
        }
    }
}
