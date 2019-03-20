using PRPR.BooruViewer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

namespace PRPR.BooruViewer.Services
{
    public class PersonalizationHelper
    {
        public static async Task SetLockScreen(Post post, Rect cropRect)
        {
            Uri urlToDownload;
            if (false) // TODO: detect whether the device is a phone
            {
                // Only select file smaller than 2MB as the system required
                if (post.FileSize <= 2000000)
                {
                    urlToDownload = new Uri(post.FileUrl);
                }
                else if (post.JpegFileSize <= 2000000)
                {
                    urlToDownload = new Uri(post.JpegUrl);
                }
                else if (post.SampleFileSize <= 2000000)
                {
                    urlToDownload = new Uri(post.SampleUrl);
                }
                else
                {
                    urlToDownload = new Uri(post.PreviewUrl);
                }
            }
            else
            {
                urlToDownload = new Uri(post.JpegUrl);
            }

            // Download the file into local storage
            // And delete other existing old file
            var imageSaved = await DownloadImageFromUri(urlToDownload, BackgroundTaskType.LockScreen);


            // Crop if needed
            //if (cropRect)
            //{
                await CropBitmap.GetCroppedBitmapAsync(imageSaved, cropRect, 1);
            //}

            // Set the lockScreen
            await ChangeLockScreenBackground(imageSaved);
        }


        public static async Task SetWallPaper(Post post)
        {
            Uri urlToDownload;
            if (false) // TODO: detect whether the device is a phone
            {
                // Only select file smaller than 2MB as the system required
                if (post.FileSize <= 2000000)
                {
                    urlToDownload = new Uri(post.FileUrl);
                }
                else if (post.JpegFileSize <= 2000000)
                {
                    urlToDownload = new Uri(post.JpegUrl);
                }
                else if (post.SampleFileSize <= 2000000)
                {
                    urlToDownload = new Uri(post.SampleUrl);
                }
                else
                {
                    urlToDownload = new Uri(post.PreviewUrl);
                }
            }
            else
            {
                urlToDownload = new Uri(post.FileUrl);
            }

            // Download the file into local storage
            // And delete other existing old file
            var imageSaved = await DownloadImageFromUri(urlToDownload, BackgroundTaskType.WallPaper);
            
            // Set the lockScreen
            await ChangeWallPaper(imageSaved);
        }
        
        public static async Task<StorageFile> DownloadImageFromUri(Uri uri, BackgroundTaskType type)
        {
            // Save the image to storage
            Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
            var referer = uri.ToString();
            client.DefaultRequestHeaders.Add("Referer", referer);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (IE 11.0; Windows NT 6.3; Trident/7.0; .NET4.0E; .NET4.0C; rv:11.0) like Gecko");


            var downloadTask = client.GetBufferAsync(uri);
            var resultBuffer = (await downloadTask);
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            var imageFolder = await localFolder.CreateFolderAsync(type == BackgroundTaskType.LockScreen ? "Lockscreens" : "Wallpapers", CreationCollisionOption.OpenIfExists);


            var imageFiles = await imageFolder.GetFilesAsync();
            foreach (var oldImageFile in imageFiles)
            {
                try
                {
                    await oldImageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch (Exception ex)
                {

                }
            }
            var imageFile = await imageFolder.CreateFileAsync(DateTime.UtcNow.Ticks.ToString() + ".jpg", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(imageFile, resultBuffer);
            return imageFile;
        }



        public static async Task<bool> ChangeWallPaper(StorageFile imageFile)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                return await settings.TrySetWallpaperImageAsync(imageFile);
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> ChangeLockScreenBackground(StorageFile imageFile)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                return await settings.TrySetLockScreenImageAsync(imageFile);
            }
            else
            {
                return false;
            }
        }



        



        public enum BackgroundTaskType
        {
            LockScreen = 0,
            WallPaper = 1,
        }
    }
}
