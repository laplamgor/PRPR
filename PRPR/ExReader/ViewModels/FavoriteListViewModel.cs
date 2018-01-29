using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.ViewModels
{
    public class FavoriteListViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        private ExFavoriteList _favoriteList = null;

        public ExFavoriteList FavoriteList
        {
            get
            {
                return _favoriteList;
            }

            set
            {
                _favoriteList = value;
                NotifyPropertyChanged(nameof(FavoriteList));
            }
        }


        private int _favoriteSortingModeSelectedIndex = 0;


        public async Task UpdateFavoriteListAsync()
        {
            FavoriteList = await ExFavoriteList.DownloadFavoritesAsync(1, FavoriteSortingModeSelectedIndex == 1? ExFavoriteSortingMode.Published : ExFavoriteSortingMode.Favorited);
        }

        public int FavoriteSortingModeSelectedIndex
        {
            get
            {
                return _favoriteSortingModeSelectedIndex;
            }

            set
            {
                _favoriteSortingModeSelectedIndex = value;
                NotifyPropertyChanged(nameof(FavoriteSortingModeSelectedIndex));
            }
        }


    }
}
