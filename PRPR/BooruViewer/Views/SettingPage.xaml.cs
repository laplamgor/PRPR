using Microsoft.EntityFrameworkCore;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
using PRPR.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
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
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }

        

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Change default download folder
            var savePicker = new FolderPicker();
            savePicker.FileTypeFilter.Add("*");
            var newDefaultFolder = await savePicker.PickSingleFolderAsync();
            if (newDefaultFolder != null)
            {
                YandeSettings.Current.DefaultDownloadPath = newDefaultFolder.Path;
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("DefaultDownloadFolder", newDefaultFolder);
            }
        }

        bool IsYandereButtonEnabled
        {
            get
            {
                return YandeSettings.Current.Host != "https://yande.re";
            }
        }

        bool IsKonachanButtonEnabled
        {
            get
            {
                return YandeSettings.Current.Host != "https://konachan.com";
            }
        }


        private async void YandereButton_Click(object sender, RoutedEventArgs e)
        {
            var host = "https://yande.re";
            var passwordHashSalt = "choujin-steiner--your-password--";
            await ChangeHostAsync(host, passwordHashSalt);
        }

        private async void KonachanButton_Click(object sender, RoutedEventArgs e)
        {
            var host = "https://konachan.com";
            var passwordHashSalt = "So-I-Heard-You-Like-Mupkids-?--your-password--";
            await ChangeHostAsync(host, passwordHashSalt);
        }

        private static async Task ChangeHostAsync(string host, string passwordHashSalt)
        {

            // Log out the user account from the current moebooru site
            YandeClient.SignOut();

            // Clear all wallpaper/lockscreen records as they are only identified with postId
            using (var db = new AppDbContext())
            {
                db.Database.ExecuteSqlCommand($"delete from {nameof(AppDbContext.WallpaperRecords)}; delete from {nameof(AppDbContext.LockScreenRecords)}");
            }

            // Change the site settings
            YandeSettings.Current.Host = host;
            YandeSettings.Current.PasswordHashSalt = passwordHashSalt;

            // Close the app
            AppRestartFailureReason result = await CoreApplication.RequestRestartAsync("");
        }
    }
}
