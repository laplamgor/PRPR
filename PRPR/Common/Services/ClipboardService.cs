using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace PRPR.Common.Services
{
    public class ClipboardService
    {
        public static async Task CopyImageAsync(IBuffer imageBuffer)
        {
            var clipboardType = typeof(DataPackage).GetTypeInfo().Assembly.GetType("Windows.ApplicationModel.DataTransfer.Clipboard");
            if (clipboardType != null)
            {
                var dataPackage = new DataPackage();
                using (var imageStream = imageBuffer.AsStream().AsRandomAccessStream())
                {
                    // Decode the image
                    var imageDecoder = await BitmapDecoder.CreateAsync(imageStream);

                    // Re-encode the image at 50% width and height
                    var inMemoryStream = new InMemoryRandomAccessStream();
                    var imageEncoder = await BitmapEncoder.CreateForTranscodingAsync(inMemoryStream, imageDecoder);
                    await imageEncoder.FlushAsync();

                    dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(inMemoryStream));
                    Clipboard.SetContent(dataPackage);
                }
            }
        }

        public static void CopyText(string text)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
