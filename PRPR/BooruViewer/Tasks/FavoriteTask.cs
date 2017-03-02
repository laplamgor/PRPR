using Microsoft.QueryStringDotNET;
using PRPR.BooruViewer.Models.Global;
using PRPR.BooruViewer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace PRPR.BooruViewer.Tasks
{
    public class FavoriteTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var d = taskInstance.GetDeferral();

            // Get the id
            ToastNotificationActionTriggerDetail details = (ToastNotificationActionTriggerDetail)taskInstance.TriggerDetails;
            var id = QueryString.Parse(details.Argument)["id"];
            await RunAsync(int.Parse(id));

            d.Complete();
        }

        private async Task RunAsync(int id)
        {
            // Favorite the post with the id
            var y = YandeSettings.Current;

            await YandeClient.VoteAsync(id, y.UserName, y.PasswordHash, VoteType.Favorite);
        }
    }
}
