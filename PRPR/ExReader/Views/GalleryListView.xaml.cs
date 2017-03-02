using PRPR.Common;
using PRPR.Common.Models;
using PRPR.ExReader.Models;
using PRPR.ExReader.Services;
using PRPR.ExReader.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.ExReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryListView : UserControl
    {
        public GalleryListView()
        {
            this.InitializeComponent();

        }



        public GalleryListViewModel GalleryListViewModel
        {
            get
            {
                return this.DataContext as GalleryListViewModel;
            }
        }

        public TestList TestList
        {
            get
            {
                return null;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(GalleryPage), (e.ClickedItem as ImageWallItem<ExGallery>).ItemSource.Link);
        }

        private bool isFirstLoad = true;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if (isFirstLoad)
            //{
            //    isFirstLoad = false;

            //    try
            //    {
            //        GalleryListViewModel.Key = "";
            //        await GalleryListViewModel.Load();
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}

        }


        private async void SearchKeyTextBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            BrowseWall.Focus(FocusState.Pointer);
            await GalleryListViewModel.Load();


            // TODO: move to VM

            //var gList = await ExGalleryList.DownloadGalleryListAsync(0, $"http://exhentai.org/?f_search={ WebUtility.UrlEncode(this.GalleryListViewModel.Key)}&{this.GalleryListViewModel.SearchConfig.ToString()}");
            //BrowseWall.DataContext = new ImageWallRows<ExGallery>();
            //(BrowseWall.DataContext as ImageWallRows<ExGallery>).RowWidth = BrowseWall.ActualWidth - BrowseWall.Padding.Left - BrowseWall.Padding.Right;
            //(BrowseWall.DataContext as ImageWallRows<ExGallery>).RowHeight = BrowseWall.ActualWidth > 500 ? 300 : 150;
            //(BrowseWall.DataContext as ImageWallRows<ExGallery>).ItemsSource = new FilteredCollection<ExGallery, ExGalleryList>(gList, this.GalleryListViewModel.GalleryFilter);

        }

        private void CategoryFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterCategoryFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }

        private void FilterReturnItem_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterMainFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }

        private void LanguageFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterLanguageFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }

        private void MinRatingFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterMinRatingFlyout"] as Flyout);
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


    public class TestList : ObservableCollection<string>, ISupportIncrementalLoading
    {

        public bool HasMoreItems
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            throw new NotImplementedException();
        }
    }


    
}
