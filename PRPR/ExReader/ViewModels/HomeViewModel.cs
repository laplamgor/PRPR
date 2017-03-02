using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


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
