using Microsoft.QueryStringDotNET;
using PRPR.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace PRPR.BooruViewer.Tasks
{
    public class LockscreenUndoTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var d = taskInstance.GetDeferral();

            // Get the id
            ToastNotificationActionTriggerDetail details = (ToastNotificationActionTriggerDetail)taskInstance.TriggerDetails;
            var id = QueryString.Parse(details.Argument)["id"];
            await RunAsync(id);

            d.Complete();
        }

        private async Task RunAsync(string id)
        {
            // Undo the bg
            ToastService.ToastDebug("Sorry", "This function is not finished yet.");
        }
    }
}
