using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public ImageViewModel()
        {

        }

        public ImageViewModel(ExGalleryImageListItem listItem)
        {
            _listItem = listItem;
            _image = new ExImage() { Link = _listItem.Link, Thumb = _listItem.Thumb };
        }

        ExGalleryImageListItem _listItem;

        public ExGalleryImageListItem ListItem
        {
            get
            {
                return _listItem;
            }
            set
            {
                _listItem = value;
                NotifyPropertyChanged(nameof(ListItem));
            }
        }

        ExImage _image = null;

        public ExImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                NotifyPropertyChanged(nameof(Image));
            }
        }
    }
}
