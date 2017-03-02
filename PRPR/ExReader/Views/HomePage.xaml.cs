using PRPR.ExReader.Models;
using PRPR.ExReader.Services;
using PRPR.BooruViewer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PRPR.Common;
using PRPR.ExReader.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.ExReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();


            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;


        }


        #region NavigationHelper

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            bool ResumingExistingPage = e.PageState != null && e.PageState.ContainsKey("Tags");

            
            if (ResumingExistingPage)
            {
                // Re-search the tags if needed
                if (GalleryListView.GalleryListViewModel.Key != e.PageState["Tags"] as string)
                {
                    GalleryListView.GalleryListViewModel.Key = e.PageState["Tags"] as string;
                    this.HomeViewModel.SelectedViewIndex = (int)(e.PageState["Tab"]);
                    try
                    {
                        await GalleryListView.GalleryListViewModel.Load();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else // Newly entered a page
            {

                if (!String.IsNullOrEmpty(e.NavigationParameter as string))
                {
                    // Turn to the searching selection
                    this.HomeViewModel.SelectedViewIndex = 0;
                }

                GalleryListView.GalleryListViewModel.Key = e.NavigationParameter as string;
                try
                {
                    await GalleryListView.GalleryListViewModel.Load();
                }
                catch (Exception ex)
                {

                }
            }


            //if (YandeSettings.Current.UserName != "")
            //{
            //    Posts favoritePost = new Posts();
            //    try
            //    {
            //        favoritePost = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags=vote:3:{YandeSettings.Current.UserName}+order:vote");
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    FavoriteWall.DataContext = new ImageWallRows<Post>();
            //    (FavoriteWall.DataContext as ImageWallRows<Post>).RowWidth = FavoriteWall.ActualWidth - FavoriteWall.Padding.Left - FavoriteWall.Padding.Right;
            //    (FavoriteWall.DataContext as ImageWallRows<Post>).RowHeight = FavoriteWall.ActualWidth > 500 ? 300 : 150;
            //    (FavoriteWall.DataContext as ImageWallRows<Post>).PostsSource = new PostsFiltered(favoritePost, (o => true));
            //}
        }


        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["Tags"] = GalleryListView.GalleryListViewModel.Key;// SearchBox.Text;
            e.PageState["Tab"] = this.HomeViewModel.SelectedViewIndex;
            
        }




        public HomeViewModel HomeViewModel
        {
            get
            {
                return this.DataContext as HomeViewModel;
            }
        }


        

        private void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }
    }
}
