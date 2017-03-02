using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Imaging;

namespace PRPR.BooruViewer.Services
{
    public class CropBitmap
    {
        public static async Task GetCroppedBitmapAsync(StorageFile originalImageFile, Rect cropRect, double scale)
        {
            if (double.IsNaN(scale) || double.IsInfinity(scale))
            {
                scale = 1;
            }
            
            // Convert start point and size to integer.
            uint startPointX = (uint)Math.Floor(cropRect.X * scale);
            uint startPointY = (uint)Math.Floor(cropRect.Y * scale);
            uint height = (uint)Math.Floor(cropRect.Height * scale);
            uint width = (uint)Math.Floor(cropRect.Width * scale);

            
            using (IRandomAccessStream stream = await originalImageFile.OpenAsync(FileAccessMode.ReadWrite))
            {

                // Create a decoder from the stream. With the decoder, we can get 
                // the properties of the image.
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                // The scaledSize of original image.
                uint scaledWidth = (uint)Math.Floor(decoder.PixelWidth * scale);
                uint scaledHeight = (uint)Math.Floor(decoder.PixelHeight * scale);
                

                // Refine the start point and the size. 
                if (startPointX + width > scaledWidth)
                {
                    startPointX = scaledWidth - width;
                }

                if (startPointY + height > scaledHeight)
                {
                    startPointY = scaledHeight - height;
                }

                // Get the cropped pixels.
                byte[] pixels = await GetPixelData(decoder, startPointX, startPointY, width, height, scaledWidth, scaledHeight);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                            (uint)width, (uint)height, 96.0, 96.0, pixels);
                
                await encoder.FlushAsync();
            }
        }




        public static Rect GetCenterCropRect(Size imageSize, Size requiredSize)
        {
            bool isImageTooWide = imageSize.Width / imageSize.Height > requiredSize.Width / requiredSize.Height;
            if (isImageTooWide)
            {
                // Crop Middle
                var finalWidth = requiredSize.Width / requiredSize.Height * imageSize.Height;
                return new Rect((imageSize.Width - finalWidth) / 2, 0, finalWidth, imageSize.Height);
            }
            else
            {
                // Crop Middle
                var finalHeight = requiredSize.Height / requiredSize.Width * imageSize.Width;
                return new Rect(0, (imageSize.Height - finalHeight) / 2, imageSize.Width, finalHeight);
            }
        }


        public static Rect GetTopMiddleCropRect(Size imageSize, Size requiredSize)
        {
            bool isImageTooWide = imageSize.Width / imageSize.Height > requiredSize.Width / requiredSize.Height;
            if (isImageTooWide)
            {
                // Crop Middle
                var finalWidth = requiredSize.Width / requiredSize.Height * imageSize.Height;
                return new Rect((imageSize.Width - finalWidth) / 2, 0, finalWidth, imageSize.Height);
            }
            else
            {
                // Crop Top
                return new Rect(0, 0, imageSize.Width, requiredSize.Height / requiredSize.Width * imageSize.Width);
            }
        }



        public static Rect GetGreedyCropRect(Size imageSize, Size requiredSize, IEnumerable<Rect> faces)
        {
            // No face
            if (faces == null || faces.Count() == 0)
            {
                // Just crop top middle. 
                return GetTopMiddleCropRect(imageSize, requiredSize);
            }


            bool isImageTooWide = imageSize.Width / imageSize.Height > requiredSize.Width / requiredSize.Height;
            if (isImageTooWide)
            {
                // Crop Left and Right
                List<Rect> sortedFace = faces.OrderByDescending(o => o.Width).ToList();
                var finalWidth = imageSize.Height * requiredSize.Width / requiredSize.Height;

                while (sortedFace.Count > 0)
                {
                    bool canFitAllFaces = sortedFace.Max(o => o.Right) - sortedFace.Min(o => o.Left) <= finalWidth;
                    if (canFitAllFaces)
                    {
                        return OptimizedCropH(imageSize, requiredSize, sortedFace);
                    }
                    else
                    {
                        // Remove the smallest(least significant) face and try again
                        sortedFace.RemoveAt(sortedFace.Count - 1);
                    }
                }

                var biggestFace = faces.OrderByDescending(o => o.Width).First();
                return CropAroundCenter(imageSize, requiredSize, new Point(biggestFace.Left + biggestFace.Width / 2, biggestFace.Top + biggestFace.Height / 2));
            }
            else
            {
                // Crop Top and Button
                List<Rect> sortedFace = faces.OrderByDescending(o => o.Height).ToList();
                var finalHeight = imageSize.Width * requiredSize.Height / requiredSize.Width;

                while (sortedFace.Count > 0)
                {
                    bool canFitAllFaces = sortedFace.Max(o => o.Bottom) - sortedFace.Min(o => o.Top) <= finalHeight;
                    if (canFitAllFaces)
                    {
                        return OptimizedCropV(imageSize, requiredSize, sortedFace);
                    }
                    else
                    {
                        // Remove the smallest(least significant) face and try again
                        sortedFace.RemoveAt(sortedFace.Count - 1);
                    }
                }

                var biggestFace = faces.OrderByDescending(o => o.Height).First();
                return CropAroundCenter(imageSize, requiredSize, new Point(biggestFace.Left + biggestFace.Width / 2, biggestFace.Top + biggestFace.Height / 2));
            }
        }

        private static Rect CropAroundCenter(Size imageSize, Size requiredSize, Point point)
        {
            bool isImageTooWide = imageSize.Width / imageSize.Height > requiredSize.Width / requiredSize.Height;
            if (isImageTooWide)
            {

                var finalWidth = imageSize.Height * requiredSize.Width / requiredSize.Height;

                return new Rect(point.X - finalWidth / 2, 0, finalWidth, imageSize.Height);
            }
            else
            {
                var finalHeight = imageSize.Width * requiredSize.Height / requiredSize.Width;

                return new Rect(0, point.Y - finalHeight / 2, imageSize.Width, finalHeight);
            }
        }

        private static Rect OptimizedCropH(Size imageSize, Size requiredSize, List<Rect> sortedFace)
        {
            var left = sortedFace.Min(o => o.Left);
            var right = sortedFace.Max(o => o.Right);
            var finalWidth = imageSize.Height * requiredSize.Width / requiredSize.Height;

            var leftSpace = left;
            var rightSpace = imageSize.Width - right;

            var leftRightRatio = leftSpace / (leftSpace + rightSpace);

            var leftFinalSpace = left - ( finalWidth - (right - left) )* leftRightRatio;

            return new Rect(leftFinalSpace, 0, finalWidth, imageSize.Height);
        }


        private static Rect OptimizedCropV(Size imageSize, Size requiredSize, List<Rect> sortedFace)
        {
            var top = sortedFace.Min(o => o.Top);
            var botton = sortedFace.Max(o => o.Bottom);
            var finalHeight = imageSize.Width * requiredSize.Height / requiredSize.Width;

            var topSpace = top;
            var bottomSpace = imageSize.Height - botton;

            var topBottomRatio = topSpace / (topSpace + bottomSpace);

            var topFinalSpace = top - (finalHeight - (botton - top)) * topBottomRatio;

            return new Rect(0, topFinalSpace, imageSize.Width, finalHeight);
        }




        /// <summary>
        /// Use BitmapTransform to define the region to crop, and then get the pixel data in the region
        /// </summary>
        private static async Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY,
            uint width, uint height)
        {
            return await GetPixelData(decoder, startPointX, startPointY, width, height,
                decoder.PixelWidth, decoder.PixelHeight);
        }

        /// <summary>
        /// Use BitmapTransform to define the region to crop, and then get the pixel data in the region.
        /// If you want to get the pixel data of a scaled image, set the scaledWidth and scaledHeight
        /// of the scaled image.
        /// </summary>
        private static async Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY,
            uint width, uint height, uint scaledWidth, uint scaledHeight)
        {
            BitmapTransform transform = new BitmapTransform();
            BitmapBounds bounds = new BitmapBounds();
            bounds.X = startPointX;
            bounds.Y = startPointY;
            bounds.Height = height;
            bounds.Width = width;
            transform.Bounds = bounds;

            transform.ScaledWidth = scaledWidth;
            transform.ScaledHeight = scaledHeight;

            // Get the cropped pixels within the bounds of transform.
            PixelDataProvider pix = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb);
            byte[] pixels = pix.DetachPixelData();
            return pixels;
        }

        


    }
}
