using PRPR.Common;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Services;
using PRPR.BooruViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Documents;
using Windows.Storage.Provider;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using System.Net;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.QueryStringDotNET;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.BooruViewer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        public ImagePage()
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


        public static Stack<ObservableCollection<Post>> PostDataStack { get; } = new Stack<ObservableCollection<Post>>();


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                // Remove a level of post list data from stack
                if (PostDataStack.Count > 0)
                {
                    PostDataStack.Pop();
                }


                // Prepare backward transition connected animation
                // Pre-fall creator has different image loading order
                // unable to share same connected animation code without breaking the UI
                if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
                {
                    if (FlipView.SelectedIndex >= 0)
                    {
                        try
                        {
                            var container = FlipView.ContainerFromIndex(FlipView.SelectedIndex) as FlipViewItem;
                            var mainPreview = container.ContentTemplateRoot as Grid;
                            var srollViewer = mainPreview.Children.First() as ScrollViewer;
                            var imageGrid = srollViewer.Content as Grid;

                            // Prepare backward connected animation
                            var grid = VisualTreeHelper.GetChild(FlipView, 0) as Grid;
                            var scrollingHost = grid.Children.FirstOrDefault(o => o is ScrollViewer) as UIElement;
                            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("PreviewImage", imageGrid);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

        }

        #endregion


        public ImagesViewModel ImagesViewModel
        {
            get
            {
                return this.DataContext as ImagesViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
            Debug.WriteLine("NavigationHelper_LoadState");
            
            try
            {
                readyForConnectedAnimation = false;

                // Get Posts
                if (ImagePage.PostDataStack.Peek() != null)
                {
                    if (this.ImagesViewModel.Posts !=  ImagePage.PostDataStack.Peek() as ObservableCollection<Post>)
                    {
                        this.ImagesViewModel = new ImagesViewModel(ImagePage.PostDataStack.Peek() as ObservableCollection<Post>);
                    }
                }
                else
                {
                    if (e.NavigationParameter != null)
                    {
                        this.ImagesViewModel = new ImagesViewModel(Post.FromXml(e.NavigationParameter as string));
                    }
                }


                if (e.PageState != null && e.PageState.ContainsKey("Index"))
                {
                    // The page is resuming, go to the index where the user was leaving
                    indexFromLastPage = (int)e.PageState["Index"];
                }
                else
                {
                    // This page is a new page navigated from search list
                    var post1 = Post.FromXml(e.NavigationParameter as string);
                    if (this.ImagesViewModel.Posts == null)
                    {
                        indexFromLastPage = 0;
                    }
                    else
                    {
                        indexFromLastPage = this.ImagesViewModel.Posts.IndexOf(this.ImagesViewModel.Posts.First(o => o.Id == post1.Id));
                    }
                }
                readyForConnectedAnimation = indexFromLastPage == FlipView.SelectedIndex;

                if (indexFromLastPage == 0)
                {
                }
                else
                {
                    this.ImagesViewModel.SelectedIndex = -1;
                    FlipView.SelectedIndex = -1;
                }
                
                this.ImagesViewModel.SelectedIndex = indexFromLastPage;


                if (ImagesViewModel.SelectedImageViewModel != null)
                {
                    await ImagesViewModel.SelectedImageViewModel.UpdateIsFavorited();
                    await ImagesViewModel.SelectedImageViewModel.UpdateComments();
                }
            }
            catch (Exception ex)
            {

            }


            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    await statusBar.HideAsync();
                }
            }
        }

        private void HandleConnectedAnimation()
        {
            // Pre-fall creator has different image loading order
            // unable to share same connected animation code without breaking the UI
            if (!ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
            {
                return;
            }


            try
            {
                var container = FlipView.ContainerFromIndex(FlipView.SelectedIndex) as FlipViewItem;


                // Do not play the animation if the target flipview item is not actually in the middle 
                // even if the SelectedIndex is pointing it, there are chance that the UI component is still not in the correct position
                var transform = container.TransformToVisual(FlipView);
                Point absolutePosition = transform.TransformPoint(new Point(0, 0));
                if ((int)absolutePosition.X != 0)
                {
                    return;
                }


                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("PreviewImage");
                if (animation != null)
                {
                    if (FlipView.SelectedIndex >= 0)
                    {
                        var mainPreview = container.ContentTemplateRoot as Grid;
                        var srollViewer = mainPreview.Children.First() as ScrollViewer;
                        var imageGrid = srollViewer.Content as Grid;
                        // Prepare backward connected animation
                        if (!animation.TryStart(imageGrid))
                        {
                            readyForConnectedAnimation = false;
                        }
                        else
                        {
                            readyForConnectedAnimation = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (this.ImagesViewModel.SelectedImageViewModel != null)
            {
                e.PageState["PostId"] = ImagesViewModel.SelectedImageViewModel.Post.Id;
                e.PageState["Index"] = ImagesViewModel.SelectedIndex;
            }
            
            


            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    await statusBar.ShowAsync();
                }
            }
        }




        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://yande.re/post/show/{ImagesViewModel.SelectedImageViewModel.Post.Id}"));
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scrollViewer = (sender as ScrollViewer);
            (scrollViewer.Content as FrameworkElement).MaxHeight = e.NewSize.Height;
            (scrollViewer.Content as FrameworkElement).MaxWidth = e.NewSize.Width;
        }




        private async void DownloadSampleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ImagesViewModel.SelectedImageViewModel.SaveImageFileAsync(PostImageVersion.Sample);
                SaveFlyout.Hide();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ImagesViewModel.SelectedImageViewModel.SaveImageFileAsync(PostImageVersion.Source);
                SaveFlyout.Hide();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }

        private async void DownloadJpegButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ImagesViewModel.SelectedImageViewModel.SaveImageFileAsync(PostImageVersion.Jpeg);
                SaveFlyout.Hide();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }










        private async void SetWallPaperButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await PersonalizationHelper.SetWallPaper(this.ImagesViewModel.SelectedImageViewModel.Post);
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }



        private void SetLockScreenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LockScreenPreviewPage), this.ImagesViewModel.SelectedImageViewModel.Post.ToXml());
        }




        private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ImagesViewModel.SelectedImageViewModel.Favorite();
            }
            catch (Exception ex)
            {

            }
        }

        private async void UnfavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            await ImagesViewModel.SelectedImageViewModel.Unfavorite();
        }



        ConnectedAnimation animation = null;


        private void PreviewImage_ImageOpened(object sender, RoutedEventArgs e)
        {
        }

        private void SampleImage_ImageOpened(object sender, RoutedEventArgs e)
        {
        }

        private void JpegImage_ImageOpened(object sender, RoutedEventArgs e)
        {
        }





        bool ButtonsOverlaying = true;
        bool DetailsOverlaying = false;
        private void MainPreview_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("MainPreview_Tapped");


            // Toggle the buttons display
            if (!ButtonsOverlaying)
            {
                ButtonsOverlaying = true;
                var b = VisualStateManager.GoToState(CurrentImagePage, "ButtonsOnly", true);
            }
            else
            {
                if (DetailsOverlaying)
                {
                    DetailsOverlaying = false;
                    var b = VisualStateManager.GoToState(CurrentImagePage, "ButtonsOnly", true);
                }
                else
                {
                    ButtonsOverlaying = false;
                    var b = VisualStateManager.GoToState(CurrentImagePage, "NoOverlay", true);
                }
            }


            e.Handled = true;
        }


        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (DetailsOverlaying)
            {
                DetailsOverlaying = false;
                var b = VisualStateManager.GoToState(CurrentImagePage, "ButtonsOnly", true);
            }
            else
            {
                DetailsOverlaying = true;
                var b = VisualStateManager.GoToState(CurrentImagePage, "Details", true);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://yande.re/post/show/{ImagesViewModel.SelectedImageViewModel.Post.Id}"));
        }


        private void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            // Search tags
            this.Frame.Navigate(typeof(HomePage), $"user:{this.ImagesViewModel.SelectedImageViewModel.Post.Author}");
        }

        private void TagsWrapBlock_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshTagsWrapBlock(sender as Panel);
        }

        private void TagsWrapBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            RefreshTagsWrapBlock(sender as Panel);
        }


        void RefreshTagsWrapBlock(Panel tagsWrapPanel)
        {
            tagsWrapPanel.Children.Clear();
            var post = this.ImagesViewModel.SelectedImageViewModel?.Post;
            if (post != null)
            {
                var groupedTags = this.ImagesViewModel.SelectedImageViewModel.Post.TagItems.GroupBy(o => o.Type);
                foreach (var group in groupedTags)
                {
                    var gOrdered = group.OrderBy(o => o.Name.Length);
                    foreach (var item in gOrdered)
                    {
                        var button = new ContentControl()
                        {
                            ContentTemplate = this.Resources["TagButtonTemplate"] as DataTemplate,
                            DataContext = item,
                            Transitions = new TransitionCollection { new RepositionThemeTransition() }
                        };
                        tagsWrapPanel.Children.Add(button);
                    }
                }
            }
            else
            {

            }
        }


        private void TagButton_Click(object sender, RoutedEventArgs e)
        {

            // Search tags
            this.Frame.Navigate(typeof(HomePage), $"{((sender as Button).DataContext as TagDetail).Name}");
        }



        private void ImageScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Debug.WriteLine("ImageScrollViewer_DoubleTapped");

            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer.ZoomFactor != 1)
            {
                scrollViewer.ChangeView(null, null, 1, true);
            }
            else
            {
                var point2 = e.GetPosition(scrollViewer);


                var point = e.GetPosition(scrollViewer.Content as FrameworkElement);


                var bw = scrollViewer.ActualWidth;
                var bh = scrollViewer.ActualHeight;
                var iw = (scrollViewer.Content as FrameworkElement).ActualWidth;
                var ih = (scrollViewer.Content as FrameworkElement).ActualHeight;



                double? y = point.Y;
                double? x = point.X;
                if (bw / bh > iw / ih)
                {
                    if (iw * 2 < bw)
                    {
                        x = null;
                    }
                    else
                    {
                        x = point.X * 2 - (bw / 2 - (iw / 2 - point.X));

                    }
                }
                else
                {
                    if (ih * 2 < bh)
                    {
                        y = null;
                    }
                    else
                    {
                        y = point.Y * 2 - (bh / 2 - (ih / 2 - point.Y));
                    }
                }

                scrollViewer.ChangeView(x, y, 2, true);
            }

            e.Handled = true;
        }





        private async void CopyPreviewMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ImagesViewModel.SelectedImageViewModel.CopyImagesAsync(PostImageVersion.Preview);
        }

        private async void CopySampleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ImagesViewModel.SelectedImageViewModel.CopyImagesAsync(PostImageVersion.Sample);
        }

        private async void CopyJpegMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ImagesViewModel.SelectedImageViewModel.CopyImagesAsync(PostImageVersion.Jpeg);
        }

        private async void CopySourceMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ImagesViewModel.SelectedImageViewModel.CopyImagesAsync(PostImageVersion.Source);
        }



        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var x = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
            

            var transform = (sender as Grid).TransformToVisual((sender as Grid).Parent as ScrollViewer);
            Point absolutePosition = transform.TransformPoint(new Point(0, 0));

            dummyGrid.Margin = new Thickness(e.GetPosition(sender as FrameworkElement).X + absolutePosition.X, e.GetPosition(sender as FrameworkElement).Y + absolutePosition.Y, 0, 0);
            x.ShowAt(dummyGrid);

            e.Handled = true;
        }

        private void Image_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                var x = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);

                var transform = (sender as Grid).TransformToVisual((sender as Grid).Parent as ScrollViewer);
                Point absolutePosition = transform.TransformPoint(new Point(0, 0));

                dummyGrid.Margin = new Thickness(e.GetPosition(sender as FrameworkElement).X, e.GetPosition(sender as FrameworkElement).Y + absolutePosition.Y, 0, 0);
                x.ShowAt(dummyGrid);

                e.Handled = true;
            }
        }

        private void PreviewImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }


        bool readyForConnectedAnimation = false;
        int indexFromLastPage = -1;


        private async void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine($"Index={ImagesViewModel.SelectedIndex}, {(sender as FlipView).SelectedIndex}");


            FlipView.UpdateLayout();
            if (FlipView.SelectedIndex == indexFromLastPage && readyForConnectedAnimation)
            {
                HandleConnectedAnimation();
            }

            // Increamental load when needed (e.g. last 3)
            if (ImagesViewModel.SelectedIndex >= ImagesViewModel.Images.Count - 10)
            {
                uint newItemCount = 0;
                while (ImagesViewModel.Posts != null && ImagesViewModel.Posts is ISupportIncrementalLoading && (ImagesViewModel.Posts as ISupportIncrementalLoading).HasMoreItems && newItemCount == 0)
                {
                    var result = await (ImagesViewModel.Posts as ISupportIncrementalLoading).LoadMoreItemsAsync(10);
                    newItemCount = result.Count;
                }
            }


            if (ImagesViewModel.SelectedImageViewModel != null)
            {
                var tasks = new List<Task>();
                tasks.Add(ImagesViewModel.SelectedImageViewModel.UpdateIsFavorited());
                tasks.Add(ImagesViewModel.SelectedImageViewModel.UpdateComments());

                //await Task.WhenAll(tasks);
            }
        }

        private void FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            FlipView.UpdateLayout();
            if (FlipView.SelectedIndex == indexFromLastPage)
            {
                HandleConnectedAnimation();
            }
        }

        private void ImageScrollViewer_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // Reset the scroll and zoom of the image
            (sender as ScrollViewer).ZoomToFactor(1);
        }
    }
}
