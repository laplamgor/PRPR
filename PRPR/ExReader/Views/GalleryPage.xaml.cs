using Microsoft.QueryStringDotNET;
using PRPR.Common;
using PRPR.Common.Models;
using PRPR.ExReader.Models;
using PRPR.ExReader.Services;
using PRPR.ExReader.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.ExReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryPage : Page
    {
        public GalleryPage()
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



        public GalleryViewModel GalleryViewModel
        {
            get
            {
                return this.DataContext as GalleryViewModel;
            }
        }


        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            try
            {
                if (GalleryViewModel.Gallery == null || GalleryViewModel.Gallery.Count == 0 || (e.NavigationParameter as string) != GalleryViewModel.Gallery?.Link)
                {
                    this.GalleryViewModel.Gallery = await ExGallery.DownloadGalleryAsync(e.NavigationParameter as string, 1, 3);


                    var f = new ImageWallRows<ExGalleryImageListItem>();
                    f.ItemsSource = this.GalleryViewModel.Gallery;
                    this.GalleryViewModel.GalleryImages = f;
                    //GalleryWall.DataContext = f;
                }
            }
            catch (Exception ex)
            {
                var x = ex.GetType();
            }
        }


        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to image page
            App.Current.Resources["Gallery"] = this.GalleryViewModel.Gallery;

            var clicked = (ImageWallItem<ExGalleryImageListItem>)(e.ClickedItem);
            

            var q = new QueryString();
            q.Add("link", this.GalleryViewModel.Gallery.Link);
            q.Add("page", $"{this.GalleryViewModel.GalleryImages.ItemsSource.IndexOf(clicked.ItemSource)}");
            

            this.Frame.Navigate(typeof(ReadingPage), q.ToString());
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            // Add to favorite
            await ExClient.AddToFavorite(GalleryViewModel.Gallery.Gid, GalleryViewModel.Gallery.Token, FavoriteCatalogComboBox.SelectedIndex, FavoriteNoteTextBox.Text);

            GalleryViewModel.IsFavorited = true;

            FavoriteAppBarButton.Flyout.Hide();

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Remove from favorite
            await ExClient.RemoveFromFavorite(GalleryViewModel.Gallery.Gid);

            GalleryViewModel.IsFavorited = false;
        }

        private void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {


        }

        private void TabListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void TagButton_Click(object sender, RoutedEventArgs e)
        {

            // Search tags
            this.Frame.Navigate(typeof(HomePage), $"{((sender as Button).DataContext as ExTag).GetQueryName()}");
        }

        private void TagsWrapBlock_Loaded(object sender, RoutedEventArgs e)
        {

            RefreshTagsWrapBlock(sender as RichTextBlock);
        }

        private void TagsWrapBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            RefreshTagsWrapBlock(sender as RichTextBlock);
        }


        void RefreshTagsWrapBlock(RichTextBlock tagsWrapBlock)
        {
            tagsWrapBlock.Blocks.Clear();
            if (this.GalleryViewModel?.Gallery?.Tags == null)
            {
                return;
            }

            var groupedTags = this.GalleryViewModel.Gallery.Tags.GroupBy(o => o.Slave);
            foreach (var group in groupedTags)
            {
                var p = new Paragraph();

                var gOrdered = group.OrderBy(o => o.Name.Length);
                //var zipped = gOrdered.Take(gOrdered.Count() / 2).Zip(gOrdered.Skip(gOrdered.Count() / 2).Reverse(), (f, s) => new List<TagDetail>() { f, s }).SelectMany(i => i);
                foreach (var item in gOrdered)
                {
                    var button = new ContentControl() { ContentTemplate = this.Resources["TagButtonTemplate"] as DataTemplate, DataContext = item };
                    var i = new InlineUIContainer() { Child = button };
                    p.Inlines.Add(i);
                }
                tagsWrapBlock.Blocks.Add(p);
            }
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








        const string EX_GALLERIES_FOLDER_NAME = "Gallery";

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GalleryViewModel?.Gallery == null)
            {
                return;
            }



            var savePicker = new FolderPicker();
            //savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeFilter.Add("*");
            var galleryParentFolder = await savePicker.PickSingleFolderAsync();
            if (galleryParentFolder == null)
            {
                return;
            }
            var galleryFolder = await galleryParentFolder.CreateFolderAsync(this.GalleryViewModel.Gallery.Gid, CreationCollisionOption.OpenIfExists);
            //var galleriesFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(EX_GALLERIES_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
            //var galleryFolder = await galleriesFolder.CreateFolderAsync(this.GalleryViewModel.Gallery.Gid, CreationCollisionOption.OpenIfExists);






            // Download image list
            await this.GalleryViewModel.Gallery.LoadAllItemsAsync();


            BackgroundTransferCompletionGroup completionGroup = new BackgroundTransferCompletionGroup();

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = "DownloadFinished";
            builder.SetTrigger(completionGroup.Trigger);
            BackgroundTaskRegistration taskRegistration = builder.Register();

            BackgroundDownloader downloader = new BackgroundDownloader(completionGroup);
            downloader.TransferGroup = BackgroundTransferGroup.CreateGroup($"{this.GalleryViewModel.Gallery.Gid}");
            downloader.TransferGroup.TransferBehavior = BackgroundTransferBehavior.Parallel;
            

            // Create tasks and file for each pic
            StorageFile[] files = new StorageFile[this.GalleryViewModel.Gallery.Count];
            foreach (var image in this.GalleryViewModel.Gallery)
            {
                files[this.GalleryViewModel.Gallery.IndexOf(image)] = await galleryFolder.CreateFileAsync($"{this.GalleryViewModel.Gallery.IndexOf(image) + 1}.jpg", CreationCollisionOption.ReplaceExisting);
            }

            // Get the image uri and download data for each pic
            List<Task> getImageUriTasks = new List<Task>();
            var gallery = this.GalleryViewModel.Gallery;
            foreach (var image in gallery)
            {
                getImageUriTasks.Add(Download(image, files[gallery.IndexOf(image)], downloader));
            }
            await Task.WhenAll(getImageUriTasks);

            
            downloader.CompletionGroup.Enable();
            
            await new MessageDialog("You can exit the app now. There will be a toast notification when all downloads are finished.", "Download started").ShowAsync();
        }

        private async Task Download(ExGalleryImageListItem image, StorageFile file, BackgroundDownloader downloader)
        {
            Debug.WriteLine(image.Link);
            
            var htmlSource = await ExClient.GetStringWithExCookie($"{image.Link}");
            Uri uri = new Uri(ExImage.GetImageUriFromHtml(htmlSource));
            DownloadOperation download = downloader.CreateDownload(uri, file);

            //Task<DownloadOperation> startTask = download.StartAsync().AsTask();
            download.StartAsync();
        }

        private void TestAppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }


























    public class WrapPanel : Panel
    {

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation",
            typeof(Orientation), typeof(WrapPanel), null);

        public WrapPanel()
        {
            // default orientation
            Orientation = Orientation.Horizontal;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(availableSize.Width, availableSize.Height));
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Point point = new Point(0, 0);
            int i = 0;

            if (Orientation == Orientation.Horizontal)
            {
                double largestHeight = 0.0;

                foreach (UIElement child in Children)
                {
                    child.Arrange(new Rect(point, new Point(point.X +
                        child.DesiredSize.Width, point.Y + child.DesiredSize.Height)));

                    if (child.DesiredSize.Height > largestHeight)
                        largestHeight = child.DesiredSize.Height;

                    point.X = point.X + child.DesiredSize.Width;

                    if ((i + 1) < Children.Count)
                    {
                        if ((point.X + Children[i + 1].DesiredSize.Width) > finalSize.Width)
                        {
                            point.X = 0;
                            point.Y = point.Y + largestHeight;
                            largestHeight = 0.0;
                        }
                    }
                    i++;
                }
            }
            else
            {
                double largestWidth = 0.0;

                foreach (UIElement child in Children)
                {
                    child.Arrange(new Rect(point, new Point(point.X +
                        child.DesiredSize.Width, point.Y + child.DesiredSize.Height)));

                    if (child.DesiredSize.Width > largestWidth)
                        largestWidth = child.DesiredSize.Width;

                    point.Y = point.Y + child.DesiredSize.Height;

                    if ((i + 1) < Children.Count)
                    {
                        if ((point.Y + Children[i + 1].DesiredSize.Height) > finalSize.Height)
                        {
                            point.Y = 0;
                            point.X = point.X + largestWidth;
                            largestWidth = 0.0;
                        }
                    }

                    i++;
                }
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
