using Microsoft.Toolkit.Uwp.Notifications;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Controls;
using PRPR.Common;
using PRPR.Common.Models.Global;
using PRPR.Common.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Notifications;

namespace PRPR.BooruViewer.Services
{
    public class AnimePersonalization
    {

        public const string TILE_FOLDER_NAME = "Tile";
        public const string WALLPAPER_FOLDER_NAME = "Wallpaper";
        public const string LOCKSCREEN_FOLDER_NAME = "Lockscreen";

        public static void ResetTile()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        public static async Task SetTileAsync(FilteredCollection<Post, Posts> posts)
        {
            List<string> faces = new List<string>();
            int postPointer = 0;

            // Remove all old face file first
            try
            {
                var imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(TILE_FOLDER_NAME, CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception ex)
            {
                
            }

            // Download up to 20 faces
            while (faces.Count < 20 && ((posts.Count - 1 > postPointer) || (posts.HasMoreItems)))
            {
                if ((posts.Count - 1 < postPointer) && (posts.HasMoreItems))
                {
                    // Need to download more posts data
                    await posts.LoadMoreItemsAsync(1);
                }
                else
                {
                    // No need to download more posts data, read next post directly
                    Post post = await GetPostAtAsync(posts, postPointer);
                    var previewBuffer = await (new Windows.Web.Http.HttpClient()).GetBufferAsync(new Uri(post.PreviewUrl));
                    var rects = (await DetechFromBufferAsync(previewBuffer, 5, 55)).Faces;
                    await SaveFacesForTileAsync(rects, previewBuffer, post.Id);

                    foreach (var rect in rects)
                    {
                        faces.Add($"{post.Id}-{rects.IndexOf(rect)}.jpg");
                    }

                    postPointer++;
                }
            }

            // Update the rile notification
            if (true)
            {
                UpdateTile(faces);
            }
        }
        
        public static async Task<string> SetBackgroundImageAsync(FilteredCollection<Post, Posts> posts, uint postShuffle, CropMethod method, Size screenSize, bool isLockscreen, int qualityLevel)
        {
            var folderName = isLockscreen ? LOCKSCREEN_FOLDER_NAME : WALLPAPER_FOLDER_NAME;

            Random rnd = new Random();
            int pointer = rnd.Next((int)postShuffle);

            // Load as many post as possible until reaching the pointer
            while ((pointer >= posts.Count) && (posts.HasMoreItems))
            {
                await posts.LoadMoreItemsAsync(1);
            }

            if (posts.Count == 0)
            {
                // No result at all, cannot update background
                return "";
            }

            if (posts.Count - 1 < pointer)
            {
                // Not enough posts to shuffle, shuffle for a lower amount of posts
                pointer = rnd.Next((int)posts.Count - 1);
            }

            // Select a post
            var post = posts[pointer];

            var previewBuffer = await (new Windows.Web.Http.HttpClient()).GetBufferAsync(new Uri(post.PreviewUrl));
            var largeBufferUrl = "";
            int largeWidth = 0;
            int largeHeight = 0;
            switch (qualityLevel)
            {
                case 2:
                    if (!String.IsNullOrEmpty(post.FileUrl))
                    {
                        largeBufferUrl = post.FileUrl;
                        largeWidth = post.Width;
                        largeHeight = post.Height;
                    }
                    else
                    {
                        largeBufferUrl = post.JpegUrl;
                        largeWidth = post.JpegWidth;
                        largeHeight = post.JpegHeight;
                    }
                    break;
                case 1:
                    largeBufferUrl = post.JpegUrl;
                    largeWidth = post.JpegWidth;
                    largeHeight = post.JpegHeight;
                    break;
                default:
                    largeBufferUrl = post.SampleUrl;
                    largeWidth = post.SampleWidth;
                    largeHeight = post.SampleHeight;
                    break;
            }
            var largeBuffer = await (new Windows.Web.Http.HttpClient()).GetBufferAsync(new Uri(largeBufferUrl));


            var imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            var imageFile = await imageFolder.CreateFileAsync($"{post.Id}.jpg", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(imageFile, largeBuffer);
            var jpegFile = await imageFolder.CreateFileAsync($"{post.Id}-original.jpg", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(jpegFile, largeBuffer);



            // Crop the image
            var imageSize = new Size(largeWidth, largeHeight);
            await CropImageFile(imageFile, imageSize, method, screenSize, previewBuffer);

            if (UserProfilePersonalizationSettings.IsSupported())
            {
                if (isLockscreen)
                {
                    var b = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(imageFile);
                    if (b)
                    {
                        return post.Id.ToString();
                    }
                }
                else
                {
                    var b = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(imageFile);
                    if (b)
                    {
                        return post.Id.ToString();
                    }
                }

            }
            return "";
        }

        private static async Task CropImageFile(StorageFile imageFile, Size imageSize, CropMethod method, Size screenSize, IBuffer previewBuffer)
        {
            switch (method)
            {
                case CropMethod.None:
                    break;
                case CropMethod.Center:
                    {
                        var cropRect = CropBitmap.GetCenterCropRect(imageSize, screenSize);
                        await CropBitmap.GetCroppedBitmapAsync(imageFile, cropRect, 1.0);
                    }
                    break;
                case CropMethod.TopMiddle:
                    {
                        var cropRect = CropBitmap.GetTopMiddleCropRect(imageSize, screenSize);
                        await CropBitmap.GetCroppedBitmapAsync(imageFile, cropRect, 1.0);
                    }
                    break;
                case CropMethod.MostFaces:
                case CropMethod.BiggestFace:
                    var result = await DetechFromBufferAsync(previewBuffer, 3, 25);
                    var faces = ImageCropper.ScaleFaces(result.Faces, imageSize, result.FrameSize);
                    if (faces.Count() > 0)
                    {
                        if (method == CropMethod.BiggestFace)
                        {
                            faces = faces.OrderByDescending(o => o.Width).Take(1);
                        }
                        var cropRect = CropBitmap.GetGreedyCropRect(imageSize, screenSize, faces);
                        await CropBitmap.GetCroppedBitmapAsync(imageFile, cropRect, 1.0);
                    }
                    else
                    {
                        // No face detected, fall back crop
                        var cropRect = CropBitmap.GetTopMiddleCropRect(imageSize, screenSize);
                        await CropBitmap.GetCroppedBitmapAsync(imageFile, cropRect, 1.0);
                    }
                    break;
                default:
                    break;
            }
        }
        



        private static void UpdateTile(List<string> faceFileNames)
        {
            var peopleTile9 = new TileBindingContentPeople();
            var peopleTile15 = new TileBindingContentPeople();
            var peopleTile20 = new TileBindingContentPeople();

            foreach (var faceFileName in faceFileNames)
            {
                if (peopleTile9.Images.Count < 9)
                {
                    peopleTile9.Images.Add(new TileBasicImage() { Source = $"ms-appdata:///local/{TILE_FOLDER_NAME}/{faceFileName}" });
                }

                if (peopleTile15.Images.Count < 15)
                {
                    peopleTile15.Images.Add(new TileBasicImage() { Source = $"ms-appdata:///local/{TILE_FOLDER_NAME}/{faceFileName}" });
                }

                if (peopleTile20.Images.Count < 20)
                {
                    peopleTile20.Images.Add(new TileBasicImage() { Source = $"ms-appdata:///local/{TILE_FOLDER_NAME}/{faceFileName}" });
                }
            }


            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Content = peopleTile9
                    },
                    TileWide = new TileBinding()
                    {
                        Content = peopleTile15
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = peopleTile20
                    }
                }
            };

            TileNotification tileNotification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private static async Task SaveFacesForTileAsync(List<Rect> faces, IBuffer imageBuffer, int postId)
        {
            foreach (var face in faces)
            {
                var imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(TILE_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
                var imageFile = await imageFolder.CreateFileAsync($"{postId}-{faces.IndexOf(face)}.jpg", CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteBufferAsync(imageFile, imageBuffer);

                await CropBitmap.GetCroppedBitmapAsync(imageFile, face, 1);
            }
            
        }

        private static async Task<Post> GetPostAtAsync(FilteredCollection<Post, Posts> posts, int postPointer)
        {
            while ((postPointer >= posts.Count) && (posts.HasMoreItems))
            {
                await posts.LoadMoreItemsAsync(1);
            }

            return posts[postPointer];
        }

        private static async Task<DetechResult> DetechFromBufferAsync(IBuffer buffer, int minNeighbors = 5, Single minSize = 30)
        {
            // Detect the face(s)
            var c = new Imaging.AnimeFaceDetector();
            BitmapDecoder bd = await BitmapDecoder.CreateAsync(buffer.AsStream().AsRandomAccessStream());
            BitmapFrame bf = await bd.GetFrameAsync(0);
            List<Rect> rects = new List<Rect>();

            await Task.Run(async () =>
            {
                var s = await c.DetectBitmap(bf, 1.05, minNeighbors, new Size(minSize, minSize));
                rects = s.ToList();
            });

            return new DetechResult() {Faces = rects, FrameSize = new Size(bf.PixelWidth, bf.PixelHeight) };
        }
        
    }

    public class DetechResult
    {
        public List<Rect> Faces;
        public Size FrameSize;
    }

    public enum CropMethod
    {
        None = 0,
        TopMiddle,
        Center,
        BiggestFace,
        MostFaces,
    }
}
