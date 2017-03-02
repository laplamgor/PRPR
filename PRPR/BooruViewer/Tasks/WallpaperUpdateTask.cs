using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
using PRPR.Common.Models.Global;
using PRPR.Common.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace PRPR.BooruViewer.Tasks
{
    public class WallpaperUpdateTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var d = taskInstance.GetDeferral();
            await RunAsync();
            d.Complete();
        }

        public static async Task RunAsync()
        {
            var y = YandeSettings.Current;
            var a = AppSettings.Current;
            var key = y.WallpaperUpdateTaskSearchKey;
            var shuffle = y.WallpaperUpdateTaskShuffleCount;

            var old = y.WallpaperUpdateTaskCurrentImageFileUri;
            y.WallpaperUpdateTaskCurrentImageID = "";

            try
            {
                // Search for posts
                var posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={ WebUtility.UrlEncode(key) }");

                // Set wallpaper
                var result = await AnimePersonalization.SetBackgroundImageAsync(posts, shuffle, y.WallpaperUpdateTaskCropMethod, a.ScreenSize, false);

                // Notice the app that the wallpapaer was changed
                y.WallpaperUpdateTaskCurrentImageID = result;

                // Motice user about the change
                BigImageToast(result, y.WallpaperUpdateTaskCurrentImageFileUri, y.AvatarUri);
            }
            catch (Exception ex)
            {
                y.WallpaperUpdateTaskCurrentImageID = old;
                ToastService.ToastDebug("Cannot Update Wallpaper", ex.StackTrace);
                ToastService.ToastDebug("Cannot Update Wallpaper", ex.Message);
            }
        }


        public static void BigImageToast(string id, string imageUri, string avatarUri)
        {
            ToastContent toastContent = new ToastContent();
            toastContent.Launch = "Toast";

            toastContent.Actions = new ToastActionsCustom()
            {
                Buttons =
                {
                    new ToastButton("Favorite", new QueryString()
                    {
                        { "action", "favorite" },
                        { "id", id }
                    }.ToString())
                    {
                        ActivationType = ToastActivationType.Background
                    },

                    new ToastButton("Undo", new QueryString()
                    {
                        { "action", "undoWallpaper" },
                        { "id", id }
                    }.ToString()) { ActivationType = ToastActivationType.Background },

                    new ToastButton("Dismiss", "dismiss") { ActivationType = ToastActivationType.Protocol },
                }
            };

            toastContent.Visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = "Your wallpaper is updated!",
                            HintStyle = AdaptiveTextStyle.Header
                        },

                        new AdaptiveText()
                        {
                            Text = "Happy?",
                            HintStyle = AdaptiveTextStyle.Body
                        },

                        //new AdaptiveImage()
                        //{
                        //    Source = imageUri,
                        //    HintRemoveMargin = true,
                        //    HintCrop = AdaptiveImageCrop.None,
                        //},
                    },

                    HeroImage = new ToastGenericHeroImage()
                    {
                        Source = imageUri
                    },

                    Attribution = new ToastGenericAttributionText()
                    {
                        Text = "prpr"
                    },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = "Assets/Square44x44Logo.targetsize-256_altform-unplated.png",// avatarUri,
                        HintCrop = ToastGenericAppLogoCrop.None,
                        
                    },

                    
                }
            };

            ToastNotification toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        
    }
}
