using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
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

            try
            {
                var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
                var p = await Posts.DownloadPostsAsync(1, $"https://yande.re/post.xml?tags={ WebUtility.UrlEncode(YandeSettings.Current.TileUpdateTaskSearchKey) }");
                await AnimePersonalization.SetTileAsync(p);
            }
            catch (Exception ex)
            {
                ToastService.ToastDebug("Cannot Update Tile", ex.StackTrace);
                ToastService.ToastDebug("Cannot Update Tile", ex.Message);
            }

            d.Complete();
        }
    }
}
