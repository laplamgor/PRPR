using Microsoft.Graphics.Canvas.Effects;
using PRPR.Common;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
using PRPR.BooruViewer.ViewModels;
using PRPR.BooruViewer.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using PRPR.Common.Views.Controls;
using PRPR.Common.Models;
using System.Collections;
using PRPR.Common.Services;
using Windows.UI.Xaml.Media.Animation;
using Windows.ApplicationModel.Resources;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.BooruViewer.Views
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
        
        public HomeViewModel HomeViewModel
        {
            get
            {
                return this.DataContext as HomeViewModel;
            }
        }


        
        public string AppVersion
        {
            get
            {
                var v = Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", v.Major, v.Minor, v.Build, v.Revision);
            }
        }
        
        


        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            bool ResumingExistingPage = e.PageState != null && e.PageState.ContainsKey("Tags");

            
            if (ResumingExistingPage)
            {
                // Re-search the tags if needed
                if (SearchBox.Text != e.PageState["Tags"] as string)
                {
                    SearchBox.Text = e.PageState["Tags"] as string;
                    FlipView.SelectedIndex = (int) (e.PageState["Tab"]);
                    try
                    {
                        this.HomeViewModel.Posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={WebUtility.UrlEncode(SearchBox.Text)}");

                    }
                    catch (Exception ex)
                    {
                        this.HomeViewModel.Posts = new Posts();
                    }

                    if (HomeViewModel.BrowsePosts == null)
                    {
                        var s = new ImageWallRows<Post>()
                        {
                            ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter)
                        };
                        HomeViewModel.BrowsePosts = s;
                        BrowsePanel.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);

                    }
                    else
                    {
                        HomeViewModel.BrowsePosts.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);
                        BrowsePanel.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);

                    }
                }
            }
            else // Newly entered a page
            {

                if (!String.IsNullOrEmpty(e.NavigationParameter as string))
                {
                    // Turn to the searching selection
                    this.HomeViewModel.SelectedViewIndex = 1;
                }

                SearchBox.Text = e.NavigationParameter as string;
                try
                {
                    this.HomeViewModel.Posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={WebUtility.UrlEncode(SearchBox.Text)}");
                }
                catch (Exception ex)
                {
                    this.HomeViewModel.Posts = new Posts();
                }

                var s = new ImageWallRows<Post>()
                {
                    ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter)
                };
                HomeViewModel.BrowsePosts = s;
                BrowsePanel.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);

            }


            if (YandeSettings.Current.IsLoggedIn)
            {
                Posts favoritePost = new Posts();
                try
                {
                    favoritePost = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags=vote:3:{YandeSettings.Current.UserName}+order:vote");
                }
                catch (Exception ex)
                {
                    
                }
                FavoritePanel.ItemsSource = new FilteredCollection<Post, Posts>(favoritePost, this.HomeViewModel.SearchPostFilter);
            }
        }
        
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["Tags"] = SearchBox.Text;
            e.PageState["Tab"] = FlipView.SelectedIndex;
        }

        private void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
        }





        private void GridViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var container = (sender as GridViewItem);
            if (container != null)
            {
                var root = (FrameworkElement)container.ContentTemplateRoot;
                var image = (UIElement)root.FindName("PreviewImage");

                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("PreviewImage", image);
            }

            // Add a fade out effect
            Transitions = new TransitionCollection();
            Transitions.Add(new ContentThemeTransition());


            var post = (sender as GridViewItem).DataContext as Post;
            this.Frame.Navigate(typeof(ImagePage), post.ToXml(), new SuppressNavigationTransitionInfo());
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }
        
        public void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var container = (sender as ImageWall).ContainerFromItem(e.ClickedItem) as ListViewItem;
            if (container != null)
            {
                var root = (FrameworkElement)container.ContentTemplateRoot;
                var image = (UIElement)root.FindName("PreviewImage");

                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("PreviewImage", image);
            }
            
            // Add a fade out effect
            Transitions = new TransitionCollection();
            Transitions.Add(new ContentThemeTransition());
            

            var post = (e.ClickedItem as ImageWallItem<Post>).ItemSource;
            this.Frame.Navigate(typeof(ImagePage), post.ToXml(), new SuppressNavigationTransitionInfo());
        }
        


        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterMainFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }
        
        private void MenuFlyoutSubItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterRatingFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }

        private void FilterReturnItem_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterMainFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }
        
        private async void FavoriteRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (YandeSettings.Current.UserName != "")
            {
                Posts favoritePost = null;
                try
                {
                    favoritePost = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags=vote:3:{YandeSettings.Current.UserName}+order:vote");

                }
                catch (Exception ex)
                {
                    return;
                }
                FavoritePanel.ItemsSource = new FilteredCollection<Post, Posts>(favoritePost, this.HomeViewModel.SearchPostFilter);
            }
        }
        



        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FeatureView.FeatureViewModel.Update();
            }
            catch (Exception ex)
            {
                
            }

        }

        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterRatioFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }
        

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var newTags = string.Join(" ", sender.Text.Split(' ').Reverse().Skip(1).Reverse());
            if (newTags != "")
            {
                newTags += ' ';
            }
            sender.Text = newTags + (args.SelectedItem as TagDetail).Name + ' ';
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (HomeViewModel.BrowsePosts == null)
            {
                var s = new ImageWallRows<Post>();
                
                try
                {
                    this.HomeViewModel.Posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={WebUtility.UrlEncode(SearchBox.Text)}");
                }
                catch (Exception ex)
                {

                }
                s.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);
                HomeViewModel.BrowsePosts = s;
                BrowsePanel.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);
            }
            else
            {

                try
                {
                    this.HomeViewModel.Posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={WebUtility.UrlEncode(SearchBox.Text)}");
                }
                catch (Exception ex)
                {

                }
                HomeViewModel.BrowsePosts.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);
                BrowsePanel.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);
            }

        }




        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = VisualTreeHelper.GetChild(sender as AutoSuggestBox, 0) as Grid;
            var textbox = grid.Children.First() as TextBox;
            textbox.SelectionChanged += Textbox_SelectionChanged;
        }

        private void Textbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSuggestion(SearchBox);
        }

        private void SearchBox_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = VisualTreeHelper.GetChild(sender as AutoSuggestBox, 0) as Grid;
            var textbox = grid.Children.First() as TextBox;
            textbox.SelectionChanged -= Textbox_SelectionChanged;
        }

        private void UpdateSuggestion(AutoSuggestBox sender)
        {
            try
            {
                var grid = VisualTreeHelper.GetChild(sender, 0) as Grid;
                var textbox = grid.Children.First() as TextBox;
                var pointer = textbox.SelectionStart;
                int selecetedKeyIndex = sender.Text.Take(pointer).Count(o => o == ' ');

                var tags = sender.Text.Split(' ');
                if (tags.Length >= 1 && tags[selecetedKeyIndex] != "")
                {
                    var result = TagDataBase.Search(tags[selecetedKeyIndex]);
                    sender.ItemsSource = result;
                }
                else
                {
                    sender.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //await Task.Delay(200);
                if (args.CheckCurrent())
                {
                    UpdateSuggestion(sender);
                }
            }
        }

        private void FilterHiddenPostsListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterHiddenFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }



        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            var i = sender as Image;

            var b = i.Parent as Border;
            if (b == null)
            {
                i.Opacity = 1;
                return;
            }

            var g = b.Parent as Grid;
            if (g == null)
            {
                i.Opacity = 1;
                return;
            }


            var c = g.Parent as UserControl;
            if (c == null)
            {
                i.Opacity = 1;
                return;
            }

            VisualStateManager.GoToState(c, "ImageLoaded", true);
        }




        public ObservableCollection<string> UpdateNotes
        {
            get
            {
                var loader = ResourceLoader.GetForCurrentView();
                var notes = loader.GetString("/PatchNotes/Notes").Split('@').Where(o => !String.IsNullOrEmpty(o));

                return new ObservableCollection<string>(notes);
            }
        }

    }
}
