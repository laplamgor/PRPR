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
        
        public ObservableCollection<string> UpdateNotes
        {
            get
            {
                return new ObservableCollection<string>(new string[] {
@"[9 April 2017] v0.45.0
Yandere
- 修正Bugs
Ex
- 修正Bugs
- 新介面
"
,
@"[29 March 2017] v0.43.0
Yandere
- 自動刷新鎖屏、壁紙加上過濾設置
- 修正一些自動刷新不了的問題
Ex
- 修正解析封面縮圖失敗
"
,
@"[3 Feb 2017] v0.42.0
Yandere
- 沒有改動
Ex
- 搜索結果用可變寬度的照片牆顯示
- 搜索結果按分類上色顯示
- 加入右向左翻頁選項
"
, 
@"[15 Jan 2017] v0.41.0
Yandere
- 搜索過濾選項現在會作用在收藏清表上
— 搜索過濾選項改變時自動刷新結果
Ex
- 搜索可以過濾低平均作品
- 更多界面改成繁中
"
,
@"[3 Jan 2017] v0.40.0
Yandere
- 更多界面改成繁中
Ex
- 本子資訊顯示平均評分
- 互換圖片列表和本子資訊的顯示位置
"
,
@"[30 Dec 2016] v0.39.0
Yandere
- 部份界面改成繁中
Ex
- 閱讀頁面加個雞肋的複製功能
- 修正閱讀頁面不能快速跳頁
- 可從中段位置開始閱讀
"
,
@"[14 Oct 2016] v0.38.0
Yandere
- 修正後台無法刷新鎖屏背景
- 刷新鎖屏、壁紙時顯示通知(可即時收藏圖片)
- 刷新鎖屏、壁紙加入自動剪裁選項
Ex
- 沒有改動
"
,
@"[9 Oct 2016] v0.37.0
Yandere
- Filter設置現可自動保存
- 閃退時顯示報錯訊息
- 搜索框現會建議相關性更高的標籤
- 搜索框有多個關鍵字時根據指標所在的字作建議
Ex
- 修正頭像顯示失敗(使用Eh論壇頭像)
"
,
@"[8 Oct 2016] v0.36.0
Yandere
- 修正打開應用返回不了首頁的問題
Ex
- 加入下載功能(另存)
- 加入加載圖集失敗後自動重試
- 界面小改動
"
,
@"[5 Oct 2016] v0.35.0
Yandere
- 加入後台自動刷新Lockscreen功能
- 現可不經瀏覽器直接下載圖片
Ex
- 界面小改動
"
,
@"[2 Oct 2016] v0.34.0
Yandere
- 後台自動刷新Wallpaper加入Shuffle選項
- 改善磁貼功能圖片品質
- 改善磁貼功能、首頁前三圖片的辨認面部準確度
Ex
- 現可按圖集標籤搜索
"
,
@"[1 Oct 2016] v0.33.0
Yandere
- 加入後台自動刷新Wallpaper功能
- 掃盲
Ex
- 圖集頁的縮圖列表以照片牆顯示
"
,
@"[29 Sep 2016] v0.32.0
Yandere
- 加入後台自動刷新磁貼功能
- 修正離線時閃退
Ex
- 顯示圖集標籤(暫未能按標籤搜索)
"
,
@"[24 Sep 2016] v0.31.0
Yandere
- 全面換上新的scroll bar，提升空間感
- 顯示更多圖片資料(分數、評級)
- 加入有趣的磁貼功能(手動)
- 加入過濾隱藏貼圖的搜索選項
Ex
- 顯示更多圖集資料(語言)
- 繼續改善介面
"
,
@"[23 Sep 2016] v0.30.0
Yandere
- 加入更新日誌
- 減少首面閃退機率
- 首面加入resize後網格排列動畫
- 修正首頁Top3移位BUG(resize後依然對準面部)
- 可按自己頭像查看自己在Yandere發的圖
- 可按頭像外鏈到網頁改密碼
Ex
- 閱讀介面加入快速跳轉
- 補一下介面上方工具欄跟yandere更一致
- 修正如果加載圖集失敗，返回再進不會重新加載
"
                });
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
                        var s = new ImageWallRows<Post>();
                        s.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);
                        HomeViewModel.BrowsePosts = s;
                    }
                    else
                    {
                        HomeViewModel.BrowsePosts.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);

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
                
                var s = new ImageWallRows<Post>();
                s.ItemsSource = new FilteredCollection<Post, Posts>(this.HomeViewModel.Posts, this.HomeViewModel.SearchPostFilter);
                HomeViewModel.BrowsePosts = s;
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
                var f = new ImageWallRows<Post>();
                f.ItemsSource = new FilteredCollection<Post, Posts>(favoritePost, this.HomeViewModel.SearchPostFilter);
                FavoriteWall.DataContext = f;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ExReader.Views.HomePage));
        }
        






        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }
        
        public void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var post = (e.ClickedItem as ImageWallItem<Post>).ItemSource;
            this.Frame.Navigate(typeof(ImagePage), post.ToXml());
        }



        //private double getElementPositionY(UIElement element)
        //{
        //    var trans = element.TransformToVisual(null);
        //    var point = trans.TransformPoint(new Point());
        //    return point.Y;
        //}

        


        //private int getListViewCurrentRowIndex()
        //{
        //    var listViewY = getElementPositionY(PostsRowsListView);


        //    var minOffset = double.MaxValue;
        //    int minOffsetPageIndex = -1;

        //    // Return -1 if there is no content loaded in the listview
        //    if (PostsRowsListView.Items.Count == 0)
        //    {
        //        return -1;
        //    }


        //    for (int i = 0; i < PostsRowsListView.Items.Count(); i++)
        //    {
        //        var container = PostsRowsListView.ContainerFromIndex(i);
        //        if (container != null)
        //        {
        //            if (((FrameworkElement)container).ActualHeight != 0)
        //            {
        //                var y = getElementPositionY((UIElement)container) - listViewY;
        //                if (y != -listViewY)
        //                {
        //                    // If the item is virtualized, its position Y will be 0
        //                    if (Math.Abs(y) <= minOffset)
        //                    {
        //                        minOffset = Math.Abs(y);
        //                        minOffsetPageIndex = i;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // This container is virtualized, Do nothing
        //        }
        //    }
        //    return minOffsetPageIndex;
        //}


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
                
                var f = new ImageWallRows<Post>();
                f.ItemsSource = new FilteredCollection<Post, Posts>(favoritePost, this.HomeViewModel.SearchPostFilter);
                FavoriteWall.DataContext = f;
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

    }
}
