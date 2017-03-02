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
    public class ReadingViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        private ObservableCollection<ExImage> _images = new ObservableCollection<ExImage>();


        public ObservableCollection<ExImage> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                NotifyPropertyChanged(nameof(Images));

            }
        }


        private int _currentImageIndex = 0;

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
            }
        }

        public int CurrentImageIndex
        {
            get
            {
                return _currentImageIndex;
            }

            set
            {
                _currentImageIndex = value;
                NotifyPropertyChanged(nameof(CurrentImageIndex));
                NotifyPropertyChanged(nameof(CurrentImageIndexOneBased));
            }
        }


        public int CurrentImageIndexOneBased
        {
            get
            {
                if (_currentImageIndex == -1)
                {
                    return 1;
                }
                return _currentImageIndex + 1;
            }
            set
            {
                CurrentImageIndex = value - 1;
            }
        }


        private int _imageCount = 0;
        public int ImageCount
        {
            get
            {
                return _imageCount;
            }
            set
            {
                _imageCount = value;
                NotifyPropertyChanged(nameof(ImageCount));
            }
        }
    }
}
