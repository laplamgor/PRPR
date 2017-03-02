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
using Windows.UI.Notifications;

namespace PRPR.BooruViewer.Tasks
{
    public class LockscreenUpdateTask
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
            var key = y.LockscreenUpdateTaskSearchKey;
            var shuffle = y.LockscreenUpdateTaskShuffleCount;

            var old = y.LockscreenUpdateTaskCurrentImageFileUri;
            y.LockscreenUpdateTaskCurrentImageID = "";

            try
            {
                // Search for posts
                var posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={ WebUtility.UrlEncode(key) }");

                // Set Lockscreen
                var result = await AnimePersonalization.SetBackgroundImageAsync(posts, shuffle, y.LockscreenUpdateTaskCropMethod, a.ScreenSize, true);

                // Notice the app that the wallpapaer was changed
                y.LockscreenUpdateTaskCurrentImageID = result;

                // Motice user about the change
                BigImageToast(result, y.LockscreenUpdateTaskCurrentImageFileUri, y.AvatarUri);
            }
            catch (Exception ex)
            {
                y.LockscreenUpdateTaskCurrentImageID = old;
                ToastService.ToastDebug("Cannot Update Lockscreen", ex.StackTrace);
                ToastService.ToastDebug("Cannot Update Lockscreen", ex.Message);
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
                        { "action", "undoLockscreen" },
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
                            Text = "Your lockscreen is updated!",
                            HintStyle = AdaptiveTextStyle.Header
                        },

                        new AdaptiveText()
                        {
                            Text = "Happy?",
                            HintStyle = AdaptiveTextStyle.Body
                        },
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
