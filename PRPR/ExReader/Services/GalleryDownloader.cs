using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Popups;

namespace PRPR.ExReader.Services
{
    public static class GalleryDownloader
    {

        public static async Task StartGalleryDownloadAsync(ExGallery gallery, StorageFolder galleryFolder)
        {
            // Download image list
            await gallery.LoadAllItemsAsync();


            BackgroundTransferCompletionGroup completionGroup = new BackgroundTransferCompletionGroup();

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            {
                Name = "DownloadFinished"
            };
            builder.SetTrigger(completionGroup.Trigger);
            BackgroundTaskRegistration taskRegistration = builder.Register();

            BackgroundDownloader downloader = new BackgroundDownloader(completionGroup)
            {
                TransferGroup = BackgroundTransferGroup.CreateGroup($"{gallery.Gid}")
            };
            downloader.TransferGroup.TransferBehavior = BackgroundTransferBehavior.Parallel;


            // Create tasks and file for each pic
            StorageFile[] files = new StorageFile[gallery.Count];
            foreach (var image in gallery)
            {
                files[gallery.IndexOf(image)] = await galleryFolder.CreateFileAsync($"{gallery.IndexOf(image) + 1}.jpg", CreationCollisionOption.ReplaceExisting);
            }

            // Get the image uri and download data for each pic
            List<Task> getImageUriTasks = new List<Task>();
            foreach (var image in gallery)
            {
                getImageUriTasks.Add(StartImageDownloadAsync(gallery, image, files[gallery.IndexOf(image)], downloader));
            }
            await Task.WhenAll(getImageUriTasks);


            downloader.CompletionGroup.Enable();
        }



        private static async Task StartImageDownloadAsync(ExGallery gallery, ExGalleryImageListItem image, StorageFile file, BackgroundDownloader downloader)
        {
            Debug.WriteLine(image.Link);

            var htmlSource = await ExClient.GetStringWithExCookie($"{image.Link}");
            Uri uri = new Uri(ExImage.GetImageUriFromHtml(htmlSource));
            DownloadOperation download = downloader.CreateDownload(uri, file);

            download.StartAsync();
        }

    }
}
