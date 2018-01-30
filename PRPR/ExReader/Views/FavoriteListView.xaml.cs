using PRPR.ExReader.Models;
using PRPR.ExReader.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PRPR.ExReader.Views
{
    public sealed partial class FavoriteListView : UserControl
    {
        public FavoriteListView()
        {
            this.InitializeComponent();
        }



        public FavoriteListViewModel FavoriteListViewModel
        {
            get
            {
                return this.DataContext as FavoriteListViewModel;
            }
        }


        private bool isFirstLoad = true;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(GalleryPage), (e.ClickedItem as ExFavorite).Link);
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FavoriteListViewModel.UpdateFavoriteListAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "AppBarButton_Click").ShowAsync();
            }

        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await FavoriteListViewModel.UpdateFavoriteListAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }
    }

}
