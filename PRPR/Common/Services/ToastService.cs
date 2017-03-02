using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace PRPR.Common.Services
{
    public class ToastService
    {
        public static void ToastDebug(string text0, string text1)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements.Item(0).AppendChild(toastXml.CreateTextNode(text0));
            stringElements.Item(1).AppendChild(toastXml.CreateTextNode(text1.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ')));

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }


    }
}
