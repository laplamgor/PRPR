using Microsoft.EntityFrameworkCore;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.Common;
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

        public async Task UpdateRecordsAsync()
        {
            using (var db = new AppDbContext())
            {
                Records = await db.WallpaperRecords.OrderByDescending(o => o.DateCreated).ToListAsync();
            }
        }



        public List<WallpaperRecord> Records
        {
            get
            {
                return _records;
            }
            set
            {
                _records = value;
                NotifyPropertyChanged(nameof(Records));
            }
        }

        private List<WallpaperRecord> _records;
    }
}
