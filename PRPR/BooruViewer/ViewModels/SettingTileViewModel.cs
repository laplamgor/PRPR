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
    public class SettingTileViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion



        public PostFilter TilePostFilter
        {
            get
            {
                return YandeSettings.Current.TilePostFilter;
            }

            set
            {
                YandeSettings.Current.TilePostFilter = value;
                NotifyPropertyChanged(nameof(TilePostFilter));
            }
        }
    }
}
