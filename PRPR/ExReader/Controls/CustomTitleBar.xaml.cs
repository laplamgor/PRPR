using PRPR.ExReader.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PRPR.ExReader.Controls
{
    public sealed partial class CustomTitleBar : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("UserControl_Loaded");


            // The SizeChanged event is raised when the view enters or exits full screen mode. 
            Window.Current.SizeChanged += OnWindowSizeChanged;
            coreTitleBar.LayoutMetricsChanged += OnLayoutMetricsChanged;


            UpdateLayoutMetrics();

            this.UpdatePositionAndVisibility();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("UserControl_Unloaded");



            Window.Current.SizeChanged -= OnWindowSizeChanged;
            coreTitleBar.LayoutMetricsChanged -= OnLayoutMetricsChanged;
        }

        void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            //Debug.WriteLine("OnWindowSizeChanged");
            this.UpdatePositionAndVisibility();
        }

        void UpdatePositionAndVisibility()
        {
            if (ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                // Hide the entire custom title bar.
                this.Visibility = Visibility.Collapsed;


                if (TitleBar != null)
                {
                    //TitleBar.InactiveBackgroundColor = Colors.LightGray;
                    //TitleBar.InactiveForegroundColor = Colors.Black;


                    TitleBar.ButtonBackgroundColor = null;
                    TitleBar.ButtonHoverBackgroundColor = null;
                    TitleBar.ButtonPressedBackgroundColor = null;
                    TitleBar.ButtonInactiveBackgroundColor = null;

                    TitleBar.ButtonForegroundColor = null;
                    TitleBar.ButtonHoverForegroundColor = null;
                    TitleBar.ButtonPressedForegroundColor = null;
                    TitleBar.ButtonInactiveForegroundColor = null;


                    TitleBar.BackgroundColor = null;
                    TitleBar.ForegroundColor = null;
                }

            }
            else
            {
                // The title bar is visible and does not overlay content.
                if (TitleBar != null)
                {
                    //TitleBar.InactiveBackgroundColor = (App.Current.Resources["SystemControlHighlightChromeAltLowBrush"] as SolidColorBrush).Color;
                    //TitleBar.InactiveForegroundColor = (App.Current.Resources["BackButtonBackgroundThemeBrush"] as SolidColorBrush).Color;



                    TitleBar.ButtonBackgroundColor = (App.Current.Resources["BackButtonBackgroundThemeBrush"] as SolidColorBrush).Color;
                    TitleBar.ButtonHoverBackgroundColor = (App.Current.Resources["AppBarToggleButtonPointerOverBackgroundThemeBrush"] as SolidColorBrush).Color;
                    TitleBar.ButtonPressedBackgroundColor = (App.Current.Resources["SystemControlHighlightListMediumBrush"] as SolidColorBrush).Color;
                    TitleBar.ButtonInactiveBackgroundColor = (App.Current.Resources["BackButtonBackgroundThemeBrush"] as SolidColorBrush).Color;

                    TitleBar.ButtonForegroundColor = (App.Current.Resources["SystemBaseHighColor"]) as Color?;
                    TitleBar.ButtonHoverForegroundColor = (App.Current.Resources["SystemBaseHighColor"]) as Color?;
                    TitleBar.ButtonPressedForegroundColor = (App.Current.Resources["SystemBaseHighColor"]) as Color?;
                    TitleBar.ButtonInactiveForegroundColor = (App.Current.Resources["SystemBaseHighColor"]) as Color?;


                    TitleBar.BackgroundColor = Colors.Black;
                    TitleBar.ForegroundColor = Colors.White;

                }

                if (CoreApplication.MainView != null && CoreApplication.MainView.TitleBar != null)
                {
                    CoreApplication.MainView.TitleBar.ExtendViewIntoTitleBar = true;
                }
                this.Visibility = Visibility.Visible;
            }
        }

        public Visibility BackButtonVisibility
        {
            get
            {
                return this.BackButton.Visibility;
            }
            set
            {
                this.BackButton.Visibility = value;
            }
        }


        private ApplicationViewTitleBar titleBar;

        public ApplicationViewTitleBar TitleBar
        {
            get
            {
                if (titleBar == null)
                {
                    try
                    {
                        titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return titleBar;
            }
        }

        public CustomTitleBar()
        {
            this.InitializeComponent();
        }

        void OnLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object e)
        {
            Debug.WriteLine("OnLayoutMetricsChanged");
            UpdateLayoutMetrics();
        }

        void UpdateLayoutMetrics()
        {
            this.Height = CoreTitleBarHeight;
            FullScreenGrid.Padding = CoreTitleBarPadding;

            // Clicks on the BackgroundElement will be treated as clicks on the title bar.
            Window.Current.SetTitleBar(ClickArea);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CoreTitleBarHeight"));
                PropertyChanged(this, new PropertyChangedEventArgs("CoreTitleBarPadding"));
            }
        }


        private async void FullScreenModeToggle_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("FullScreenModeToggle_Click");



            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                var succeeded = view.TryEnterFullScreenMode();
                if (!succeeded)
                {
                    var dialog = new MessageDialog("Unable to enter the full-screen mode.");
                    await dialog.ShowAsync();
                }
            }


            CoreApplication.MainView.TitleBar.ExtendViewIntoTitleBar = false;
        }


        private CoreApplicationViewTitleBar coreTitleBar = CoreApplication.MainView.TitleBar;

        public double CoreTitleBarHeight
        {
            get
            {
                return coreTitleBar.Height;
            }
        }
        public Thickness CoreTitleBarPadding
        {
            get
            {
                // The SystemOverlayLeftInset and SystemOverlayRightInset values are
                // in terms of physical left and right. Therefore, we need to flip
                // then when our flow direction is RTL.
                if (FlowDirection == FlowDirection.LeftToRight)
                {
                    return new Thickness() { Left = 0, Right = coreTitleBar.SystemOverlayRightInset };
                }
                else
                {
                    return new Thickness() { Left = coreTitleBar.SystemOverlayRightInset, Right = 0 };
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {


            if (Window.Current.Content != null)
            {
                if ((Window.Current.Content as AppShell).AppFrame.CanGoBack)
                {
                    (Window.Current.Content as AppShell).AppFrame.GoBack();
                }

            }
        }
    }
}
