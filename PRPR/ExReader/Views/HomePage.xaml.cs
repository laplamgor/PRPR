using PRPR.ExReader.Models;
using PRPR.ExReader.Services;
using PRPR.BooruViewer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PRPR.Common;
using PRPR.ExReader.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.ExReader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
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
            bool ResumingExistingPage = e.PageState != null && e.PageState.ContainsKey("ExTags");

            
            if (ResumingExistingPage)
            {
                // Re-search the tags if needed
                if (GalleryListView.GalleryListViewModel.Key != e.PageState["ExTags"] as string)
                {
                    GalleryListView.GalleryListViewModel.Key = e.PageState["ExTags"] as string;
                    this.HomeViewModel.SelectedViewIndex = (int)(e.PageState["ExTab"]);
                    try
                    {
                        await GalleryListView.GalleryListViewModel.Load();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else // Newly entered a page
            {

                if (!String.IsNullOrEmpty(e.NavigationParameter as string))
                {
                    // Turn to the searching selection
                    this.HomeViewModel.SelectedViewIndex = 0;
                }

                GalleryListView.GalleryListViewModel.Key = e.NavigationParameter as string;
                try
                {
                    await GalleryListView.GalleryListViewModel.Load();
                }
                catch (Exception ex)
                {

                }
            }
        }


        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["ExTags"] = GalleryListView.GalleryListViewModel.Key;// SearchBox.Text;
            e.PageState["ExTab"] = this.HomeViewModel.SelectedViewIndex;
        }




        public HomeViewModel HomeViewModel
        {
            get
            {
                return this.DataContext as HomeViewModel;
            }
        }


        

        private void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }
    }
}
