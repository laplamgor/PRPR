using PRPR.BooruViewer.Views;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.Common.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        public NavigationPage()
        {
            this.InitializeComponent();
        }

        private void NvSample_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            navOptions.IsNavigationStackEnabled = false;


            if (args.IsSettingsInvoked == true)
            {
                contentFrame.NavigateToType(typeof(SettingPage), null, navOptions);
            }
            else if (args.InvokedItemContainer != null)
            {

                Type pageType;
                if (args.InvokedItemContainer == SamplePage1Item)
                {
                    pageType = typeof(BooruViewer.Views.TabPage);
                }
                else if (args.InvokedItemContainer == SamplePage2Item)
                {
                    pageType = typeof(BooruViewer.Views.HomePage);
                }
                else
                {
                    pageType = typeof(ExReader.Views.HomePage);
                }
                contentFrame.NavigateToType(pageType, null, navOptions);
            }
        }
    }
}
