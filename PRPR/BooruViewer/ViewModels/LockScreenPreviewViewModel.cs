using PRPR.BooruViewer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.ViewModels
{
    public class LockScreenPreviewViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private Post _post = null;

        public Post Post
        {
            get
            {
                return _post;
            }

            set
            {
                _post = value;
                NotifyPropertyChanged(nameof(Post));
            }
        }



    }
}
