using PRPR.BooruViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PRPR.BooruViewer.Views
{
    public sealed partial class TabPostListView : UserControl
    {
        public TabPostListView()
        {
            this.InitializeComponent();
        }



        public TabPostListViewModel TabPostListViewModel
        {
            get
            {
                return this.DataContext as TabPostListViewModel;
            }
        }


        private void BrowsePanel_ItemClick(object sender, Common.Controls.JustifiedWrapPanel.ItemClickEventArgs e)
        {

        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void SearchBox_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var flyout = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
            var options = new FlyoutShowOptions()
            {
                // Position shows the flyout next to the pointer.
                // "Transient" ShowMode makes the flyout open in its collapsed state.
                //Position = e.GetPosition((FrameworkElement)sender),
                ShowMode = FlyoutShowMode.Transient
            };
            flyout?.ShowAt((FrameworkElement)sender, options);
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await TabPostListViewModel.SearchAsync(SearchBox.Text);
        }

        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BrowsePanel_ItemClick(object sender, object e)
        {

        }
    }
}
