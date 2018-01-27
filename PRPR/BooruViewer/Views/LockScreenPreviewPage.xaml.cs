using PRPR.Common;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Services;
using PRPR.BooruViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.Globalization.DateTimeFormatting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.BooruViewer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LockScreenPreviewPage : Page
    {
        public LockScreenPreviewPage()
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


        public LockScreenPreviewViewModel LockScreenPreviewViewModel
        {
            get
            {
                return this.DataContext as LockScreenPreviewViewModel;
            }
        }


        bool IsFullScreenEnabled = false;

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            try
            {
                if (e.NavigationParameter != null)
                {
                    this.LockScreenPreviewViewModel.Post = Post.FromXml(e.NavigationParameter as string);
                }
            }
            catch (Exception ex)
            {

            }



            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            IsFullScreenEnabled = ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            ApplicationView.GetForCurrentView().SuppressSystemOverlays = true;
            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar") && StatusBar.GetForCurrentView() != null)
            {
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                await StatusBar.GetForCurrentView().ShowAsync();
            }
            else
            {
                ApplicationView.GetForCurrentView().VisibleBoundsChanged += LockScreenPreviewPage_VisibleBoundsChanged;
            }

            PreviewTextBlock.Text = DateTimeOffset.Now.ToString("%H");
            PreviewTextBlock2.Text = DateTimeOffset.Now.ToString("mm");

            try
            {
                var f = DateTimeFormatter.LongDate;
                var formateWithoutYear = new DateTimeFormatter(YearFormat.None, MonthFormat.Full, DayFormat.Default, DayOfWeekFormat.Full, f.IncludeHour, f.IncludeMinute, f.IncludeSecond, Windows.System.UserProfile.GlobalizationPreferences.Languages);
                PreviewTextBlock3.Text = formateWithoutYear.Format(DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                
            }
        }

        private void LockScreenPreviewPage_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            var s = ApplicationView.GetForCurrentView();
            if (!s.IsFullScreenMode && IsFullScreenEnabled)
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
        }

        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            ApplicationView.GetForCurrentView().SuppressSystemOverlays = false;
            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;


            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar") && StatusBar.GetForCurrentView() != null)
            {
                
                    await StatusBar.GetForCurrentView().HideAsync();

                    DisplayInformation.AutoRotationPreferences = (DisplayOrientations)15;
            }

            ApplicationView.GetForCurrentView().VisibleBoundsChanged -= LockScreenPreviewPage_VisibleBoundsChanged;
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateImagePosition();
        }

        private void UpdateImagePosition()
        {
            var image = SetLockScreenPreviewImage;
            if (image.ActualWidth != 0 && image.ActualHeight != 0 && SetLockScreenPreViewScrollViewer.ActualWidth != 0 && SetLockScreenPreViewScrollViewer.ActualHeight != 0)
            {

                if (image.ActualWidth / image.ActualHeight >= SetLockScreenPreViewScrollViewer.ActualWidth / SetLockScreenPreViewScrollViewer.ActualHeight)
                {
                    image.Height = SetLockScreenPreViewScrollViewer.ActualHeight;
                    image.Width = Double.NaN;
                    // Scroll to middle
                    SetLockScreenPreViewScrollViewer.ScrollToHorizontalOffset((image.ActualWidth / image.ActualHeight * SetLockScreenPreViewScrollViewer.ActualHeight - SetLockScreenPreViewScrollViewer.ActualWidth) / 2);
                }
                else
                {

                    image.Width = SetLockScreenPreViewScrollViewer.ActualWidth;
                    image.Height = Double.NaN;
                }
            }
        }

        private void ScrollViewer_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            UpdateImagePosition();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var startx = SetLockScreenPreViewScrollViewer.HorizontalOffset * this.LockScreenPreviewViewModel.Post.JpegWidth / SetLockScreenPreviewImage.ActualWidth / SetLockScreenPreViewScrollViewer.ZoomFactor;
                var starty = SetLockScreenPreViewScrollViewer.VerticalOffset * this.LockScreenPreviewViewModel.Post.JpegHeight / SetLockScreenPreviewImage.ActualHeight / SetLockScreenPreViewScrollViewer.ZoomFactor;

                var width = SetLockScreenPreViewScrollViewer.ActualWidth * this.LockScreenPreviewViewModel.Post.JpegWidth / SetLockScreenPreviewImage.ActualWidth / SetLockScreenPreViewScrollViewer.ZoomFactor;
                var height = SetLockScreenPreViewScrollViewer.ActualHeight * this.LockScreenPreviewViewModel.Post.JpegHeight / SetLockScreenPreviewImage.ActualHeight / SetLockScreenPreViewScrollViewer.ZoomFactor;



                await PersonalizationHelper.SetLockScreen(this.LockScreenPreviewViewModel.Post, new Rect(startx, starty, width, height));


                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void BitmapImage_DownloadProgress(object sender, Windows.UI.Xaml.Media.Imaging.DownloadProgressEventArgs e)
        {
            // Update progress bar
            DownloadProgressBar.Value = e.Progress;
        }
    }


}
