using Microsoft.Toolkit.Uwp.UI.Controls;
using PRPR.BooruViewer.Models;
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

namespace PRPR.BooruViewer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabPage : Page
    {
        public TabPage()
        {
            this.InitializeComponent();
        }

        private void Tabs_Loaded(object sender, RoutedEventArgs e)
        {


            (sender as TabView).Items.Add(CreateTabViewItem(new TabSummary()));
        }


        private TabViewItem CreateTabViewItem(Tab content)
        {
            ContentControl container = new ContentControl();
            {
                container.Content = content;
                container.Style = Tabs.ItemContainerStyle;
                container.ContentTemplate = (DataTemplate)this.Resources[content.GetType().Name]; ;
            }


            var item = new TabViewItem()
            {
                Content = container
            };

            if (content is TabSummary)
            {
                item.Header = "Home";
                item.Icon = new SymbolIcon() { Symbol = Symbol.Home };
                item.IsClosable = false;
            } else if (content is TabPostList)
            {
                item.Header = "Search";
                item.Icon = new SymbolIcon() { Symbol = Symbol.Find };
            }
            else if (content is TabPostDetail)
            {
                item.Header = "Image";
                item.Icon = new SymbolIcon() { Symbol = Symbol.BrowsePhotos };
            }


            return item;
        }
    }
}
