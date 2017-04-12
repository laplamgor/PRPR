using PRPR.BooruViewer.Models;
using PRPR.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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



        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            // Download the last 24 hr tags
            var p = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags=rating:s");
            var dayBefore = p.First().created_at - 3600 * 24 * 1;
            while (p.Last().created_at >= dayBefore)
            {
                await p.LoadMoreItemsAsync(100);
            }
            var x = p.Where(o => o.created_at >= dayBefore);

            
        }


        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

    }
}
