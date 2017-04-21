using PRPR.BooruViewer.Models.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    }
}
