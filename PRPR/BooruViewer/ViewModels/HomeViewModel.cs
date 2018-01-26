using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.Common;
using PRPR.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Popups;
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


        private ObservableCollection<Post> _favoritePosts = null;

        public ObservableCollection<Post> FavoritePosts
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

        public async Task SearchAsync(string keyword)
        {
            // Validate the keyword







            // Dont search if there is more than 6 tags
            if (keyword.Split(' ').Where( o=> !String.IsNullOrWhiteSpace(o)).Count() > 6)
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                await new MessageDialog(resourceLoader.GetString("/BooruHomePage/MessageDialogTooManyTags/Content"),
                    resourceLoader.GetString("/BooruHomePage/MessageDialogTooManyTags/Title")).ShowAsync();
                return;
            }

            // Dont search if there is any rating metatag
            if (keyword.Split(' ').FirstOrDefault(o => o.Contains("rating:")) != default(String))
            {
                // Unlock the rating filter
                if (!YandeSettings.Current.IsRatingFilterUnlocked)
                {
                    YandeSettings.Current.IsRatingFilterUnlocked = true;
                }
                var resourceLoader = ResourceLoader.GetForCurrentView();
                await new MessageDialog(resourceLoader.GetString("/BooruHomePage/MessageDialogRatingTag/Content"),
                    resourceLoader.GetString("/BooruHomePage/MessageDialogRatingTag/Title")).ShowAsync();
                return;
            }




            // Updated the search results
            Posts posts;
            try
            {
                posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={WebUtility.UrlEncode(keyword)}");
            }
            catch (Exception ex)
            {
                posts = new Posts();
            }
            SearchPosts = new FilteredCollection<Post, Posts>(posts, SearchPostFilter);
        }
        

        public HomeViewModel()
        {
        }

        private List<string> FavoriteSortingMode = new List<string>()
        {
            "order:vote",
            "order:id_asc",
            "order:id",
            "order:score",
            "order:score_asc"
        };

        private int _favoriteSortingModeSelectedIndex = 0;


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

        private PostFilter _searchPostFilter = new PostFilter();


        public async Task UpdateFavoriteListAsync()
        {
            if (YandeSettings.Current.UserName != "")
            {
                Posts favoritePost = null;
                try
                {
                    string sortString = FavoriteSortingMode.First();
                    if (FavoriteSortingModeSelectedIndex >= 0 || FavoriteSortingModeSelectedIndex < FavoriteSortingMode.Count)
                    {
                        sortString = FavoriteSortingMode[FavoriteSortingModeSelectedIndex];
                    }

                    favoritePost = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags=vote:3:{YandeSettings.Current.UserName}+{sortString}");
                }
                catch (Exception ex)
                {
                    return;
                }
                this.FavoritePosts = favoritePost;
            }
        }
    }
}
