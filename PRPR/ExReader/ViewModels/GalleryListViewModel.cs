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
using Windows.UI.Popups;

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



        private FilteredCollection<ExGallery, ExGalleryList> _searchGalleryList = null;

        public FilteredCollection<ExGallery, ExGalleryList> SearchGalleryList
        {
            get
            {
                return _searchGalleryList;
            }

            set
            {
                _searchGalleryList = value;
                NotifyPropertyChanged(nameof(SearchGalleryList));
            }
        }





        public async Task Load()
        {
            var gList = await ExGalleryList.DownloadGalleryListAsync(0, $"https://exhentai.org/?f_search={ WebUtility.UrlEncode(Key)}&{SearchConfig.ToString()}");

            var gList2 = new FilteredCollection<ExGallery, ExGalleryList>(gList, this.GalleryFilter);

            SearchGalleryList = gList2;

            // Add the search record to DB if user defined keywords
            if (!String.IsNullOrWhiteSpace(Key))
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        db.ExSearchRecords.Add(ExSearchRecord.Create(Key));
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, "Fail to save search record").ShowAsync();
                }
            }
        }
    }
}
