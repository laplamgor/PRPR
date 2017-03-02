using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public sealed partial class SettingTilePage : Page
    {
        public SettingTilePage()
        {
            this.InitializeComponent();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var p = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={ WebUtility.UrlEncode(TileTextBox.Text) }");

                await AnimePersonalization.SetTileAsync(p);
            }
            catch (Exception ex)
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AnimePersonalization.ResetTile();
        }
    }
}
