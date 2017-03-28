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
    public class SettingWallpaperViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion



        public PostFilter WallpaperPostFilter
        {
            get
            {
                return YandeSettings.Current.WallpaperPostFilter;
            }

            set
            {
                YandeSettings.Current.WallpaperPostFilter = value;
                NotifyPropertyChanged(nameof(WallpaperPostFilter));
            }
        }
    }
}
