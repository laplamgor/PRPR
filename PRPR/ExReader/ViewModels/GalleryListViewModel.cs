using PRPR.Common;
using PRPR.Common.Models;
using PRPR.ExReader.Models;
using PRPR.ExReader.Models.Global;
using PRPR.ExReader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.ViewModels
{
    public class GalleryListViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private ImageWallRows<ExGallery> _galleryList = null;

        public ImageWallRows<ExGallery> GalleryList
        {
            get
            {
                return _galleryList;
            }

            set
            {
                _galleryList = value;
                NotifyPropertyChanged(nameof(GalleryList));
            }
        }
        



        public ExSearchConfig SearchConfig
        {
            get
            {
                return ExSettings.Current.SearchConfig;
            }

            set
            {
                ExSettings.Current.SearchConfig = value;
                NotifyPropertyChanged(nameof(SearchConfig));
            }
        }


        private ExConfig _config = new ExConfig();



        public string Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
                NotifyPropertyChanged(nameof(Key));
            }
        }

        public ExConfig Config
        {
            get
            {
                return _config;
            }

            set
            {
                _config = value;
                NotifyPropertyChanged(nameof(Config));
            }
        }
        
        public ExGalleryFilter GalleryFilter
        {
            get
            {
                return ExSettings.Current.GalleryFilter;
            }

            set
            {
                ExSettings.Current.GalleryFilter = value;
                NotifyPropertyChanged(nameof(this.GalleryFilter));
            }
        }


        private string _key = "";


        public async Task Load()
        {
            var gList = await ExGalleryList.DownloadGalleryListAsync(0, $"http://exhentai.org/?f_search={ WebUtility.UrlEncode(Key)}&{SearchConfig.ToString()}");

            //(this.GalleryList as ImageWallRows<ExGallery>).RowWidth = BrowseWall.ActualWidth - BrowseWall.Padding.Left - BrowseWall.Padding.Right;
            //(this.GalleryList as ImageWallRows<ExGallery>).RowHeight = BrowseWall.ActualWidth > 500 ? 300 : 150;
            var s = new ImageWallRows<ExGallery>();
            s.ItemsSource = new FilteredCollection<ExGallery, ExGalleryList>(gList, this.GalleryFilter);
            this.GalleryList = s;
        }

    }
}
