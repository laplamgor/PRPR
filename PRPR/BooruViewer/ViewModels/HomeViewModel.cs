using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.Common;
using PRPR.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PRPR.BooruViewer.ViewModels
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

        private Posts _posts = null;

        public Posts Posts
        {
            get
            {
                return _posts;
            }

            set
            {
                _posts = value;
                NotifyPropertyChanged(nameof(HomeViewModel.Posts));
            }
        }

        public PostFilter SearchPostFilter
        {
            get
            {
                return YandeSettings.Current.SearchPostFilter;
            }

            set
            {
                YandeSettings.Current.SearchPostFilter = value;
                NotifyPropertyChanged(nameof(SearchPostFilter));
            }
        }


        private FilteredCollection<Post, Posts> _favoritePosts = null;



        public FilteredCollection<Post, Posts> FavoritePosts
        {
            get
            {
                return _favoritePosts;
            }

            set
            {
                _favoritePosts = value;
                NotifyPropertyChanged(nameof(FavoritePosts));
            }
        }


        private FilteredCollection<Post, Posts> _searchPosts = null;



        public FilteredCollection<Post, Posts> SearchPosts
        {
            get
            {
                return _searchPosts;
            }

            set
            {
                _searchPosts = value;
                NotifyPropertyChanged(nameof(SearchPosts));
            }
        }



        public ImageWallRows<Post> BrowsePosts
        {
            get
            {
                return _browsePosts;
            }

            set
            {
                _browsePosts = value;
                NotifyPropertyChanged(nameof(BrowsePosts));
            }
        }


        public HomeViewModel()
        {
        }

        private List<string> FavoriteSortingMode = new List<string>() { "order:vote", "order:id", "order:id_asc" };

        private PostFilter _searchPostFilter = new PostFilter();

       
        private ImageWallRows<Post> _browsePosts = null;
    }
}
