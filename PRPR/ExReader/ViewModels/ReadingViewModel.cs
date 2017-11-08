using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public ReadingViewModel()
        {

        }

        public ReadingViewModel(ExGallery gallery)
        {
            _gallery = gallery;
            _gallery.CollectionChanged += _gallery_CollectionChanged;

            // Create vm for each previewed image
            foreach (var item in _gallery)
            {
                _imageViewModels.Add(new ImageViewModel(item));
            }

            // Add dummy vm for all un-previewed images
            while (_imageViewModels.Count < gallery.FileCount)
            {
                _imageViewModels.Add(new ImageViewModel());
            }
        }

        private void _gallery_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Update vm for newly previewed image if it was an dummy item
                foreach (ExGalleryImageListItem item in e.NewItems)
                {
                    var index = _gallery.IndexOf(item);
                    if (_imageViewModels[index].ListItem == null)
                    {
                        // Replace the dummy vm with a new one
                        _imageViewModels[index] = new ImageViewModel(item);
                    }
                }
            }
        }

        private ObservableCollection<ImageViewModel> _imageViewModels = new ObservableCollection<ImageViewModel>();

        public ObservableCollection<ImageViewModel> ImageViewModels
        {
            get => _imageViewModels;
            set
            {
                _imageViewModels = value;
                NotifyPropertyChanged(nameof(ImageViewModels));
            }
        }



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
