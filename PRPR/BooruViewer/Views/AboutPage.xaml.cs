using Imaging;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.ViewModels;
using PRPR.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.BooruViewer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
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


        AboutViewModel AboutViewModel
        {
            get
            {
                return this.DataContext as AboutViewModel;
            }
        }


        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //var inpainter = new Inpainter();

            //try
            //{

            //    InMemoryRandomAccessStream inMemoryRandomAccessStream = await inpainter.InpaintMS();

            //    Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(inMemoryRandomAccessStream.GetInputStreamAt(0));
            //    MemoryStream memoryStream = new MemoryStream();
            //    if (stream != null)
            //    {
            //        byte[] bytes = ReadFully(stream);
            //        if (bytes != null)
            //        {
            //            var binaryWriter = new BinaryWriter(memoryStream);
            //            binaryWriter.Write(bytes);
            //        }
            //    }
            //    IBuffer buffer = WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(memoryStream, 0, (int)memoryStream.Length);





            //    var s = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, 1007, 1479);
            //    //var s = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, 1007, 1479);
            //    SoftwareBitmap s2 = SoftwareBitmap.Convert(s, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            //    var ss = new SoftwareBitmapSource();
            //    await ss.SetBitmapAsync(s2);

            //    TestImage.Source = ss;
            //}
            //catch (Exception ex)
            //{

            //}

        }

        static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }


        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

        private async void TestImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //var inpainter = new Inpainter();
            //try
            //{

            //    InMemoryRandomAccessStream inMemoryRandomAccessStream = await inpainter.InpaintMS();


            //    Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(inMemoryRandomAccessStream.GetInputStreamAt(0));
            //    MemoryStream memoryStream = new MemoryStream();
            //    if (stream != null)
            //    {
            //        byte[] bytes = ReadFully(stream);
            //        if (bytes != null)
            //        {
            //            var binaryWriter = new BinaryWriter(memoryStream);
            //            binaryWriter.Write(bytes);
            //        }
            //    }
            //    IBuffer buffer = WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(memoryStream, 0, (int)memoryStream.Length);


            //    var s = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, 1007, 1479);
            //    SoftwareBitmap s2 = SoftwareBitmap.Convert(s, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            //    var ss = new SoftwareBitmapSource();
            //    await ss.SetBitmapAsync(s2);

            //    TestImage.Source = ss;
            //}
            //catch (Exception ex)
            //{

            //}

        }
    }
}

