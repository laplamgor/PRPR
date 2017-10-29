using Microsoft.QueryStringDotNET;
using PRPR.Common;
using PRPR.Common.Models;
using PRPR.Common.Services;
using PRPR.ExReader.Models;
using PRPR.ExReader.Services;
using PRPR.ExReader.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
using Windows.UI.Xaml.Media.Animation;
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
            set
            {
                this.DataContext = value;
            }
        }


        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {


            try
            {
                // Load the gallery info
                if (GalleryViewModel.Gallery == null || GalleryViewModel.Gallery.Count == 0 || (e.NavigationParameter as string) != GalleryViewModel.Gallery?.Link)
                {
                    this.GalleryViewModel = new GalleryViewModel();

                    this.GalleryViewModel.Gallery = await ExGallery.DownloadGalleryAsync(e.NavigationParameter as string, 1, 3);
                }

                
            }
            catch (Exception ex)
            {

            }
        }


        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            // Add to favorite
            await ExClient.AddToFavorite(GalleryViewModel.Gallery.Gid, GalleryViewModel.Gallery.Token, FavoriteCatalogComboBox.SelectedIndex, FavoriteNoteTextBox.Text);

            GalleryViewModel.IsFavorited = true;

            FavoriteButton.Flyout.Hide();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Remove from favorite
            await ExClient.RemoveFromFavorite(GalleryViewModel.Gallery.Gid);

            GalleryViewModel.IsFavorited = false;
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
                await new MessageDialog("Please wait until the page is loaded.", "Download failed").ShowAsync();
                return;
            }
            

            var savePicker = new FolderPicker();
            savePicker.FileTypeFilter.Add("*");
            var galleryParentFolder = await savePicker.PickSingleFolderAsync();
            if (galleryParentFolder != null)
            {
                var galleryFolder = await galleryParentFolder.CreateFolderAsync(this.GalleryViewModel.Gallery.Gid, CreationCollisionOption.OpenIfExists);
                
                await this.GalleryViewModel.StartGalleryDownloadAsync(galleryFolder);
                await new MessageDialog("You can exit the app now. There will be a toast notification when all downloads are finished.", "Download started").ShowAsync();
            }
        }


        bool DetailPanelExtended = false;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (DetailPanelExtended)
            {
                DetailPanelExtended = false;
            }
            else
            {
                DetailPanelExtended = true;
            }

            UpdateDetailPanelState(true);
        }
        

        bool isLeft = false;
        private void WidthStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == Width560Height0 || e.NewState == Width700)
            {
                // Left
                isLeft = true;
            }
            else
            {
                // Bottom
                isLeft = false;
            }

            UpdateDetailPanelState(true);
        }



        void UpdateDetailPanelState(bool useTransitions)
        {
            bool b = false;
            if (isLeft)
            {
                if (DetailPanelExtended)
                {
                    b = VisualStateManager.GoToState(CurrentGalleryPage, "LeftExtended", useTransitions);
                }
                else
                {
                    b = VisualStateManager.GoToState(CurrentGalleryPage, "LeftCollapsed", useTransitions);
                }
            }
            else
            {
                if (DetailPanelExtended)
                {
                    b = VisualStateManager.GoToState(CurrentGalleryPage, "BottomExtended", useTransitions);
                }
                else
                {
                    b = VisualStateManager.GoToState(CurrentGalleryPage, "BottomCollapsed", useTransitions);
                }
            }
            Debug.WriteLine($"left={isLeft};Extended={DetailPanelExtended};b={b}");
        }

        private void CurrentGalleryPage_Loaded(object sender, RoutedEventArgs e)
        {
            var currentState = GetCurrentState("WidthStates");
            if (currentState == Width560Height0 || currentState == Width700)
            {
                // Left
                isLeft = true;
            }
            else
            {
                // Bottom
                isLeft = false;
            }

            UpdateDetailPanelState(true);



            // Jump to the page item if this is a back button action
            if (this.Frame.CanGoForward)
            {
                var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
                var lastPageParameters = frameState["Page-" + (this.Frame.BackStackDepth + 1)] as IDictionary<string, object>;
                var index = (int)lastPageParameters["Page"];


                // Scroll into the index of last opened page
                ItemsWrapPanel.ScrollIntoView((ItemsWrapPanel.ItemsSource as IList)[index], ScrollIntoViewAlignment.Default);

                // Start the animation
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ThumbImage");
                if (animation != null)
                {
                    if (ItemsWrapPanel.ContainerFromIndex(index) is ContentControl container)
                    {
                        var root = (FrameworkElement)container.ContentTemplateRoot;
                        var image = (UIElement)root.FindName("ThumbImage");
                        animation.TryStart(image);
                    }
                }
            }
            
        }

        public VisualState GetCurrentState(string stateGroupName)
        {
            VisualStateGroup stateGroup1 = null;

            IList<VisualStateGroup> list = VisualStateManager.GetVisualStateGroups(VisualTreeHelper.GetChild(this, 0) as FrameworkElement);

            foreach (var v in list)
                if (v.Name == stateGroupName)
                {
                    stateGroup1 = v;
                    break;
                }

            return stateGroup1.CurrentState;
        }

        private void GridViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var item = (ExGalleryImageListItem)(sender as FrameworkElement).DataContext;

            // Navigate to image page
            App.Current.Resources["Gallery"] = this.GalleryViewModel.Gallery;
            
            var q = new QueryString
            {
                { "link", this.GalleryViewModel.Gallery.Link },
                { "page", $"{this.GalleryViewModel.Gallery.IndexOf(item)}" }
            };

            var container = (sender as GridViewItem);
            if (container != null)
            {
                var root = (FrameworkElement)container.ContentTemplateRoot;
                var image = (UIElement)root.FindName("ThumbImage");

                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ThumbImage", image);
            }


            this.Frame.Navigate(typeof(ReadingPage), q.ToString());
        }







        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Search tags
            this.Frame.Navigate(typeof(HomePage), $"{((sender as FrameworkElement).DataContext as ExTag).GetQueryName()}");
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            // Copy tags
            ClipboardService.CopyText($"{((sender as FrameworkElement).DataContext as ExTag).GetQueryName()}");
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var rawName = Regex.Replace(GalleryViewModel.Gallery.Title, @"(\([^\(\)]*\)|\[[^\[\]]*\])", m => "").Split('|')[0].Trim();
            // Search for other languages
            this.Frame.Navigate(typeof(HomePage), rawName);
        }








        #region Handle comment rich text block

        private void RichTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var richTextBlock = sender as RichTextBlock;
            richTextBlock.Blocks.Clear();

            if (richTextBlock.DataContext is string comment)
            {
                var lines = comment.Split('\n');
                foreach (var line in lines)
                {
                    richTextBlock.Blocks.Add(CraeteLine(line));
                }
            }
        }

        private void RichTextBlock_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var richTextBlock = sender as RichTextBlock;
            richTextBlock.Blocks.Clear();
            
            if (richTextBlock.DataContext is string comment)
            {
                var lines = comment.Split('\n');
                foreach (var line in lines)
                {
                    richTextBlock.Blocks.Add(CraeteLine(line));
                }
            }
        }

        
        // \b(?:https?://(e-hentai.org|exhentai.org))/g/\S+\b\/?
        // To find all gallery url on exhentai or ehentai, http or https

        Block CraeteLine(string line)
        {
            var paragraph = new Paragraph();
            

            var linkParser = new Regex(@"\b(?:https?://(e-hentai.org|exhentai.org))/g/\S+\b\/?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            int indexAfterPreviousMatch = 0;
            foreach (Match match in linkParser.Matches(line))
            {
                bool hasTextBetween = indexAfterPreviousMatch < match.Index;
                if (hasTextBetween)
                {
                    var textBetween = line.Substring(indexAfterPreviousMatch, match.Index - indexAfterPreviousMatch);
                    paragraph.Inlines.Add(new Run() { Text = textBetween });
                }
                
                paragraph.Inlines.Add(CraeteHyperLink(match.Value));
                indexAfterPreviousMatch = match.Index + match.Length;
            }

            bool hasTextEnd = indexAfterPreviousMatch < line.Length;
            if (hasTextEnd)
            {
                var textBetween = line.Substring(indexAfterPreviousMatch);
                paragraph.Inlines.Add(new Run() { Text = textBetween });
            }

            return paragraph;
        }

        Inline CraeteHyperLink(string link)
        {
            var hyperLink = new Hyperlink();
            hyperLink.Inlines.Add(new Run() { Text = link });
            hyperLink.Click += HyperLink_Click;
            return hyperLink;
        }

        private void HyperLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(GalleryPage), (sender.Inlines.First() as Run).Text);
        }


        #endregion

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // Post new comment
            await this.GalleryViewModel.CommentAsync();
        }



        // Just for debug
        private async void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            var d = await BackgroundDownloader.GetCurrentDownloadsForTransferGroupAsync(BackgroundTransferGroup.CreateGroup("Gallery"));
            var downloawwqwds = d.ToList();
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
