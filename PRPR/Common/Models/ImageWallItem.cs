using System.ComponentModel;

namespace PRPR.Common.Models
{
    public class ImageWallItem<T> : INotifyPropertyChanged where T : IImageWallItemImage
    {
        public T ItemSource { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private double _displayHeight = 0;

        public double DisplayWidth
        {
            get
            {
                return _displayWidth;
            }

            set
            {
                _displayWidth = value;
                NotifyPropertyChanged(nameof(DisplayWidth));
            }
        }

        public double DisplayHeight
        {
            get
            {
                return _displayHeight;
            }

            set
            {
                _displayHeight = value;
                NotifyPropertyChanged(nameof(DisplayHeight));
            }
        }

        public double PreferredWidth
        {
            get
            {
                return ItemSource.PreferredWidth;
            }
        }

        public double PreferredHeight
        {
            get
            {
                return ItemSource.PreferredHeight;
            }
        }

        private double _displayWidth = 0;
    }









    public interface IImageWallItemImage
    {
        double PreferredWidth
        {
            get;
        }

        double PreferredHeight
        {
            get;
        }


        double PreferredRatio
        {
            get;
        }
    }

}
