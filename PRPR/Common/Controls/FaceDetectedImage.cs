using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PRPR.Common.Controls
{
    class FaceDetectedImage : Control
    {

        public string ProxySource
        {
            get
            {
                return (string)GetValue(ProxySourceProperty);
            }
            set
            {
                SetValue(ProxySourceProperty, value);
            }
        }

        public string ImageSource
        {
            get
            {
                return (string)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }


        public static readonly DependencyProperty ProxySourceProperty =
            DependencyProperty.Register(nameof(ProxySource), typeof(string), typeof(FaceDetectedImage), null);// new PropertyMetadata(null, new PropertyChangedCallback(OnProxyChanged)));


        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(string), typeof(FaceDetectedImage), null);




        protected override Size MeasureOverride(Size availableSize)
        {
            return availableSize;
        }
    }
}
