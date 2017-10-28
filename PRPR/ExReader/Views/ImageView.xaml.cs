using PRPR.Common.Services;
using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PRPR.ExReader.Views
{
    public sealed partial class ImageView : UserControl
    {
        public ImageView()
        {
            this.InitializeComponent();
        }



        ExImage Image
        {
            get
            {
                return this.DataContext as ExImage;
            }
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scrollViewer = (sender as ScrollViewer);
            (scrollViewer.Content as FrameworkElement).MaxHeight = e.NewSize.Height;
            (scrollViewer.Content as FrameworkElement).MaxWidth = e.NewSize.Width;
        }


        private void BitmapImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            UpdateDownloadProcessBar(e.Progress);
        }

        void UpdateDownloadProcessBar(int process)
        {
            ProcessBar.Value = process;
            ProcessBar.Maximum = 100;
            if (process == 100)
            {
                ProcessBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                ProcessBar.Visibility = Visibility.Visible;

            }
        }



        private void ScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = (sender as ScrollViewer);


            if (scrollViewer.ZoomFactor == 1)
            {
                scrollViewer.ChangeView(e.GetPosition(scrollViewer).X, e.GetPosition(scrollViewer).Y, 2, true);
            }
            else
            {
                scrollViewer.ChangeView(null, null, 1, true);
            }
        }


        private void ScrollViewer_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // Reset the zoom if the image is changed
            var scrollViewer = (sender as ScrollViewer);
            // Reset the scroll and zoom of the image
            scrollViewer.ZoomToFactor(1);
            UpdateDownloadProcessBar(0);
        }


        















        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

            var x = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);

            dummyGrid.Margin = new Thickness(e.GetPosition(OutterGrid).X, e.GetPosition(OutterGrid).Y, 0, 0);
            x.ShowAt(dummyGrid);

            e.Handled = true;
        }

        private void Image_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                var x = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);

                dummyGrid.Margin = new Thickness(e.GetPosition(OutterGrid).X, e.GetPosition(OutterGrid).Y, 0, 0);
                x.ShowAt(dummyGrid);

                e.Handled = true;
            }
        }




        private async void CopyMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            IBuffer imageBuffer = null;

            if (this.Image != null)
            {
                var source = this.Image.ImageSource;

                var hc = new HttpClient();
                imageBuffer = await hc.GetBufferAsync(new Uri(source));
            }

            await ClipboardService.CopyImageAsync(imageBuffer);
        }
    }
}
