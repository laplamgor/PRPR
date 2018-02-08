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
using PRPR.Common.Models;
using System.Collections;
using PRPR.Common.Services;
using Windows.UI.Xaml.Media.Animation;
using Windows.ApplicationModel.Resources;
using Microsoft.QueryStringDotNET;
using PRPR.Common.Controls;
using Windows.Foundation.Metadata;

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

                    await HomeViewModel.SearchAsync(SearchBox.Text);
                }
            }
            else // Newly entered a page
            {

                if (e.NavigationParameter != null && !String.IsNullOrEmpty(e.NavigationParameter as string))
                {
                    // Turn to the searching selection
                    MainPivot.UpdateLayout();
                    this.HomeViewModel.SelectedViewIndex = 1;
                }

                SearchBox.Text = e.NavigationParameter as string;

                await HomeViewModel.SearchAsync(SearchBox.Text);
                
            }
        }
        
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["Tags"] = SearchBox.Text;
            e.PageState["Tab"] = MainPivot.SelectedIndex;
        }

        private void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var frameState = SuspensionManager.SessionStateForFrame(this.Frame);


                if (this.Frame.CanGoForward && frameState.ContainsKey("Page-" + (this.Frame.BackStackDepth + 1)))
                {


                    // Jump to the pivot where user left
                    if (frameState.ContainsKey("Page-" + (this.Frame.BackStackDepth)))
                    {
                        var thisPageParameters = frameState["Page-" + (this.Frame.BackStackDepth)] as IDictionary<string, object>;
                        if (thisPageParameters.ContainsKey("Tab") && MainPivot.SelectedIndex != (int)(thisPageParameters["Tab"]))
                        {
                            // Work around to disable to pivot turning animation by changing the index twice                            
                            MainPivot.SelectedIndex = (MainPivot.SelectedIndex + 2) % 4;
                            MainPivot.SelectedIndex = (MainPivot.SelectedIndex + 1) % 4;
                            MainPivot.SelectedIndex = (int)(thisPageParameters["Tab"]);

                            //MainPivot.UpdateLayout();
                        }
                    }


                    
                    // Jump to the page item if this is a back button action
                    var lastPageParameters = frameState["Page-" + (this.Frame.BackStackDepth + 1)] as IDictionary<string, object>;
                    if (lastPageParameters.ContainsKey("Index") && lastPageParameters.ContainsKey("PostId"))
                    {
                        var index = (int)lastPageParameters["Index"];
                        var postId = (int)lastPageParameters["PostId"];


                        if (this.HomeViewModel.SelectedViewIndex == 0)
                        {

                            // Pre-fall creator has different image loading order
                            // unable to share same connected animation code without breaking the UI
                            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
                            {

                                var post = FeatureView.FeatureViewModel.TopToday.First(o => o.Id == postId);
                                if (post != null && FeatureView.FeatureViewModel.TopToday.IndexOf(post) != -1)
                                {
                                    // Start the animation
                                    ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("PreviewImage");
                                    if (animation != null)
                                    {
                                        FeatureView.UpdateLayout();
                                        var button = this.FeatureView.GetTopTodayButton(FeatureView.FeatureViewModel.TopToday.IndexOf(post)) as Button;
                                        var image = ((UIElement)((Border)button.Content).Child) as ImageCropper;
                                        animation.TryStart(image.ImageInside);
                                    }
                                }

                            }
                        }
                        else if (this.HomeViewModel.SelectedViewIndex == 1 || this.HomeViewModel.SelectedViewIndex == 2)
                        {
                            JustifiedWrapPanel panel = null;
                            if (this.HomeViewModel.SelectedViewIndex == 1)
                            {
                                // Navigating back from a search result image
                                panel = BrowsePanel;
                            }
                            else if (this.HomeViewModel.SelectedViewIndex == 2)
                            {
                                // Navigating back from a favorite image
                                panel = FavoritePanel;
                            }

                            // Scroll into the index of last opened page
                            panel.ScrollIntoView((panel.ItemsSource as IList)[index], ScrollIntoViewAlignment.Default);
                            panel.UpdateLayout();




                            // Pre-fall creator has different image loading order
                            // unable to share same connected animation code without breaking the UI
                            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
                            {
                                // Start the animation
                                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("PreviewImage");
                                if (animation != null)
                                {
                                    if (panel.ContainerFromIndex(index) is ContentControl container)
                                    {
                                        var root = (FrameworkElement)container.ContentTemplateRoot;
                                        var image = (UIElement)root.FindName("PreviewImage");
                                        animation.TryStart(image);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
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

        private void FilterHiddenPostsListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterHiddenFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }

        private void FilterBlacklistListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Flyout.SetAttachedFlyout(FilterButton, this.Resources["FilterBlacklistFlyout"] as Flyout);
            Flyout.ShowAttachedFlyout(FilterButton);
        }


        private async void FavoriteRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await HomeViewModel.UpdateFavoriteListAsync();
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
        

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {


            await HomeViewModel.SearchAsync(SearchBox.Text);
        }




        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = VisualTreeHelper.GetChild(sender as AutoSuggestBox, 0) as Grid;
            var textbox = grid.Children.First() as TextBox;
            textbox.SelectionChanged += Textbox_SelectionChanged;
        }


        private int lastSelectionStart = 0;

        private void Textbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Re-search suggestions if the user move the cursor position to another word
            // Except the cursor is at the end, bcs it is probably caused by a SuggestionChosen event
            var newSelectionStart = (sender as TextBox).SelectionStart;
            int newSelecetedKeyIndex = (sender as TextBox).Text.Take(newSelectionStart).Count(o => o == ' ');
            int lastSelecetedKeyIndex = (sender as TextBox).Text.Take(lastSelectionStart).Count(o => o == ' ');
            if (lastSelecetedKeyIndex != newSelecetedKeyIndex && newSelectionStart != (sender as TextBox).Text.Length)
            {
                UpdateSuggestions(SearchBox);
            }
            lastSelectionStart = newSelectionStart;
        }

        private void SearchBox_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = VisualTreeHelper.GetChild(sender as AutoSuggestBox, 0) as Grid;
            var textbox = grid.Children.First() as TextBox;
            textbox.SelectionChanged -= Textbox_SelectionChanged;
        }

        private void UpdateSuggestions(AutoSuggestBox sender)
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
                    var results = TagDataBase.Search(tags[selecetedKeyIndex]);

                    // Add back the other tags to the string
                    var prefix = String.Join(" ", tags.Take(selecetedKeyIndex));
                    if (!String.IsNullOrWhiteSpace(prefix))
                    {
                        prefix += " ";
                    }
                    var suffix = String.Join(" ", tags.Skip(selecetedKeyIndex + 1));
                    if (!String.IsNullOrWhiteSpace(suffix))
                    {
                        suffix = " " + suffix;
                    }

                    // Also add an extra space at the end so that its easier to start typing next new type
                    var results2 = results.Select(o => new TagDetailInMiddle(o, prefix, suffix + " "));

                    // Display the tag search results
                    sender.ItemsSource = results2;
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
                    UpdateSuggestions(sender);
                }
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




        public ObservableCollection<string> UpdateNotes
        {
            get
            {
                var loader = ResourceLoader.GetForCurrentView();
                var notes = loader.GetString("/PatchNotes/Notes").Split('@').Where(o => !String.IsNullOrEmpty(o));

                return new ObservableCollection<string>(notes);
            }
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await HomeViewModel.UpdateFavoriteListAsync();
        }
        
        private void BrowsePanel_ItemClick(object sender, JustifiedWrapPanel.ItemClickEventArgs e)
        {
            // Clicked a list item from the image wall

            var panel = (sender as JustifiedWrapPanel);
            var container = panel.ContainerFromItem(e.ClickedItem);
            if (container != null)
            {
                var root = (container as ContentControl).ContentTemplateRoot;
                var image = (UIElement)(root as FrameworkElement).FindName("PreviewImage");

                // Pre-fall creator has different image loading order
                // unable to share same connected animation code without breaking the UI
                if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
                {
                    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("PreviewImage", image);
                }
            }

            var post = e.ClickedItem as Post;

            // Navigate to image page
            ImagePage.PostDataStack.Push(panel.ItemsSource as ObservableCollection<Post>);
            this.Frame.Navigate(typeof(ImagePage), post.ToXml(), new SuppressNavigationTransitionInfo());
        }

    }
}
