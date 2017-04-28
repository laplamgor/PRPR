using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.ViewModels;
using PRPR.ExReader.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace PRPR.BooruViewer.Views
{
    public sealed partial class FeatureView : UserControl
    {
        public FeatureView()
        {
            this.InitializeComponent();
        }

        public FeatureViewModel FeatureViewModel
        {
            get
            {
                return this.DataContext as FeatureViewModel;
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.FeatureViewModel.TopToday.Count == 0)
            {
                try
                {
                    await this.FeatureViewModel.Update();
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, "UserControl_Loaded").ShowAsync();
                }
            }
        }
        
        private void Top3_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext is Post post)
            {
                (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(ImagePage), post.ToXml());
            }
        }
        
        private async void Tags6_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            if ((sender as Button).DataContext is FeaturedTag tag)
            {
                // Search tags
                (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(HomePage), $"{tag.Name}");

            }
        }
    }
}
