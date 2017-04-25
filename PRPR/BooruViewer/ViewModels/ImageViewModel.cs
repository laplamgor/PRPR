using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
using PRPR.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace PRPR.BooruViewer.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private Post _post = null;

        public Post Post
        {
            get
            {
                return _post;
            }

            set
            {
                _post = value;
                NotifyPropertyChanged(nameof(Post));
            }
        }




        private Comments _comments = null;

        public Comments Comments
        {
            get
            {
                return _comments;
            }

            set
            {
                _comments = value;
                NotifyPropertyChanged(nameof(Comments));
            }
        }




        private bool _isFavorited = false;

        public bool IsFavorited
        {
            get
            {
                return _isFavorited;
            }

            private set
            {
                _isFavorited = value;
                NotifyPropertyChanged(nameof(IsFavorited));
            }
        }

        public async Task UpdateIsFavorited()
        {
            IsFavorited = await YandeClient.CheckFavorited(Post.Id);
        }

        public async Task Favorite()
        {
            await YandeClient.AddFavoriteAsync(Post.Id);
            this.IsFavorited = true;

        }

        public async Task Unfavorite()
        {
            await YandeClient.RemoveFavoriteAsync(Post.Id);
            this.IsFavorited = false;
        }





        private static string GetFileName(string uri)
        {
            var output = uri;
            if (output.LastIndexOf('/') >= 0)
            {
                output = output.Substring(output.LastIndexOf('/'));
            }
            if (output.LastIndexOf('0') >= 0)
            {
                output = output.Substring(0, output.LastIndexOf('.'));
            }
            return WebUtility.UrlDecode(output);
        }
        
        public async Task SaveImageFileAsync(PostImageVersion version)
        {
            switch (version)
            {
                case PostImageVersion.Preview:
                    await SaveImageFileAsync(Post.PreviewUrl, "jpg");
                    break;
                case PostImageVersion.Sample:
                    await SaveImageFileAsync(Post.SampleUrl, "jpg");
                    break;
                case PostImageVersion.Jpeg:
                    await SaveImageFileAsync(Post.JpegUrl, "jpg");
                    break;
                case PostImageVersion.Source:
                    await SaveImageFileAsync(Post.FileUrl, Post.FileExtension);
                    break;
                default:
                    break;
            }
        }

        private async Task SaveImageFileAsync(string fileUri, string fileExtension)
        {
            string type = $".{fileExtension}";


            StorageFile file = null;
            if (YandeSettings.Current.IsDefaultDownloadPathEnabled)
            {
                // Try to dget the default folder
                var newFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("DefaultDownloadFolder");
                if (newFolder != null)
                {
                    var finalName = GetFileName(fileUri).Replace("/", "") + type;
                    file = await newFolder.CreateFileAsync(finalName, CreationCollisionOption.GenerateUniqueName);
                }
            }

            // Let user to pick a new file, if there is no default folder
            if (file == null)
            {
                var savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                savePicker.FileTypeChoices.Add(type, new List<string>() { type });
                savePicker.SuggestedFileName = GetFileName(fileUri);

                file = await savePicker.PickSaveFileAsync();
            }


            // Download the image
            if (file != null)
            {
                var imageBuffer = await (new HttpClient()).GetBufferAsync(new Uri(fileUri));

                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteBufferAsync(file, imageBuffer);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (status != FileUpdateStatus.Complete)
                {
                    await new MessageDialog("File " + file.Name + " couldn't be saved.").ShowAsync();
                }
            }
        }




        public async Task CopyImagesAsync(PostImageVersion version)
        {
            string uriString = "";
            switch (version)
            {
                case PostImageVersion.Preview:
                    uriString = Post.PreviewUrl;
                    break;
                case PostImageVersion.Sample:
                    uriString = Post.SampleUrl;
                    break;
                case PostImageVersion.Jpeg:
                    uriString = Post.JpegUrl;
                    break;
                case PostImageVersion.Source:
                    uriString = Post.FileUrl;
                    break;
                default:
                    break;
            }


            var hc = new HttpClient();
            var imageBuffer = await hc.GetBufferAsync(new Uri(uriString));

            await ClipboardService.CopyImageAsync(imageBuffer);
        }


    }
}
