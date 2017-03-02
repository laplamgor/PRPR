using PRPR.Common.Models;
using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.ViewModels
{
    public class GalleryViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private ExGallery _gallery = null;

        public ExGallery Gallery
        {
            get
            {
                return _gallery;
            }

            set
            {
                _gallery = value;
                NotifyPropertyChanged(nameof(Gallery));
                NotifyPropertyChanged(nameof(IsFavorited));
            }
        }

        private ImageWallRows<ExGalleryImageListItem> _galleryImages = null;

        public ImageWallRows<ExGalleryImageListItem> GalleryImages
        {
            get
            {
                return _galleryImages;
            }

            set
            {
                _galleryImages = value;
                NotifyPropertyChanged(nameof(GalleryImages));
                NotifyPropertyChanged(nameof(IsFavorited));
            }
        }


        public bool IsFavorited
        {
            get
            {
                return Gallery == null ? false : Gallery.IsFavorited;
            }
            set
            {
                Gallery.IsFavorited = value;
                NotifyPropertyChanged(nameof(IsFavorited));
            }
        }


        private int _selectedViewIndex = 0;

        public int SelectedViewIndex
        {
            get
            {
                return _selectedViewIndex;
            }

            set
            {
                _selectedViewIndex = value;
                NotifyPropertyChanged(nameof(SelectedViewIndex));
            }
        }
    }
}
