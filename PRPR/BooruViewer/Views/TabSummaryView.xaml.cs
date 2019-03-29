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
    public sealed partial class TabSummaryView : UserControl
    {
        public TabSummaryView()
        {
            this.InitializeComponent();
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
