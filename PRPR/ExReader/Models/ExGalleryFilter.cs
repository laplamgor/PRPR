using PRPR.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PRPR.ExReader.Models
{
    public class ExGalleryFilter : IConfigableFilter<ExGallery>
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        private double _minRating = 4.0;



        public Func<ExGallery, bool> Function
        {
            get
            {
                var m = MinRating;

                return o => (o.Rating >= m);
            }
        }

        public double MinRating
        {
            get
            {
                return _minRating;
            }

            set
            {
                _minRating = value;
                NotifyPropertyChanged(nameof(MinRating));
                NotifyPropertyChanged(nameof(Function));
            }
        }
    }
}
