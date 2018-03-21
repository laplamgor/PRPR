using Microsoft.EntityFrameworkCore;
using PRPR.Common;
using PRPR.Common.Controls;
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
using System.Threading.Tasks;
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
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        
        private async void SearchKeyTextBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.Focus(FocusState.Pointer);
            await GalleryListViewModel.Load();
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

        private async void SearchKeyTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await UpdateSuggestionsAsync(sender);
            }
        }

        private async Task UpdateSuggestionsAsync(AutoSuggestBox sender)
        {
            try
            {
                using (var db = new AppDbContext())
                {

                    sender.ItemsSource = (await db.ExSearchRecords.ToListAsync()).Where(o => o.Keyword.StartsWith(sender.Text)).GroupBy(o => o.Keyword).Select(grp => grp.First()).OrderByDescending(o => o.DateCreated).ToList();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void SearchKeyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            await UpdateSuggestionsAsync(sender as AutoSuggestBox);
        }


        private void BrowsePanel_ItemClick(object sender, JustifiedWrapPanel.ItemClickEventArgs e)
        {
            (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(GalleryPage), (e.ClickedItem as ExGallery).Link);
        }
    }
}