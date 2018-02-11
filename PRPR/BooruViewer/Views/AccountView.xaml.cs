using Imaging;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
using PRPR.BooruViewer.Tasks;
using PRPR.ExReader.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PRPR.BooruViewer.Views
{
    public sealed partial class AccountView : UserControl
    {
        public AccountView()
        {
            this.InitializeComponent();
        }

        public string SignUpUrl
        {
            get
            {
                return $"{YandeClient.HOST}/user/signup";
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (YandeUserNameTextBox.Text != "" && YandePasswordBox.Password != "")
            {
                // Login to yande
                await YandeClient.SignInAsync(YandeUserNameTextBox.Text, YandePasswordBox.Password);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            YandeClient.SignOut();
        }

        private async void ChangePasswordMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Laungh the webpage

            await Windows.System.Launcher.LaunchUriAsync(new Uri($"{YandeClient.HOST}/user/change_password"));
        }

        private void UserPostMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Search tags
            (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(HomePage), $"user:{YandeSettings.Current.UserName}");
        }
        

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            (Window.Current.Content as AppShell).AppFrame.Navigate(typeof(ExReader.Views.HomePage));
        }
        
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var frame = (Window.Current.Content as AppShell).AppFrame;
            
            var index = (sender as ListView).Items.IndexOf(e.ClickedItem);
            switch (index)
            {
                case 0:
                    frame.Navigate(typeof(SettingPage));
                    break;
                case 1:
                    frame.Navigate(typeof(SettingWallpaperPage));
                    break;
                case 2:
                    frame.Navigate(typeof(SettingLockscreenPage));
                    break;
                case 3:
                    frame.Navigate(typeof(SettingTilePage));
                    break;
                case 4:
                    frame.Navigate(typeof(AboutPage));
                    break;
                default:
                    break;
            }

        }
    }
}
 