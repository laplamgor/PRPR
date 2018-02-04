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
        
        public async Task UpdateRecordsAsync()
        {
            using (var db = new AppDbContext())
            {
                Records = await db.LockScreenRecords.OrderByDescending(o => o.DateCreated).ToListAsync();
            }
        }

        

        public List<LockScreenRecord> Records
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

        private List<LockScreenRecord> _records;
    }
}
