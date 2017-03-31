using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
using PRPR.Common;
using PRPR.Common.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace PRPR.BooruViewer.Tasks
{
    public class TileUpdateTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var d = taskInstance.GetDeferral();

            await RunAsync();

            d.Complete();
        }

        public static async Task RunAsync()
        {
            var yandeSettings = YandeSettings.Current;
            var key = yandeSettings.TileUpdateTaskSearchKey;
            var filter = yandeSettings.TilePostFilter;
            
            try
            {
                var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
                var posts = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={ WebUtility.UrlEncode(key) }");

                var filteredPosts = new FilteredCollection<Post, Posts>(posts, filter);

                await AnimePersonalization.SetTileAsync(filteredPosts);
            }
            catch (Exception ex)
            {
                ToastService.ToastDebug("Cannot Update Tile", ex.StackTrace);
                ToastService.ToastDebug("Cannot Update Tile", ex.Message);
            }
        }
    }
}
