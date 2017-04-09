using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace PRPR.Common.Controls
{
    [Windows.UI.Xaml.Markup.ContentProperty(Name = "Content")]
    public sealed class RotateContentControl : Control
    {
        private ContentControl m_Content;

        public RotateContentControl()
        {
            this.DefaultStyleKey = typeof(RotateContentControl);
        }

        protected override void OnApplyTemplate()
        {
            m_Content = GetTemplateChild("Content") as ContentControl;
            base.OnApplyTemplate();
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            if (m_Content != null)
            {
                if (((int)Direction) % 180 == 90)
                {
                    m_Content.Measure(new Windows.Foundation.Size(availableSize.Height, availableSize.Width));
                    return new Size(m_Content.DesiredSize.Height, m_Content.DesiredSize.Width);
                }
                else
                {
                    m_Content.Measure(availableSize);
                    return m_Content.DesiredSize;
                }
            }
            else
                return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (m_Content != null)
            {
                m_Content.RenderTransform = new RotateTransform() { Angle = (int)this.Direction };
                if (Direction == RotateDirection.Up)
                    m_Content.Arrange(new Rect(new Point(0, finalSize.Height),
                                      new Size(finalSize.Height, finalSize.Width)));
                else if (Direction == RotateDirection.Down)
                    m_Content.Arrange(new Rect(new Point(finalSize.Width, 0),
                                      new Size(finalSize.Height, finalSize.Width)));
                else if (Direction == RotateDirection.UpsideDown)
                    m_Content.Arrange(new Rect(new Point(finalSize.Width, finalSize.Height), finalSize));
                else
                    m_Content.Arrange(new Rect(new Point(), finalSize));
                return finalSize;
            }
            else
                return base.ArrangeOverride(finalSize);
        }


        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(RotateContentControl), null);

        public enum RotateDirection : int
        {
            Normal = 0,
            Down = 90,
            UpsideDown = 180,
            Up = 270
        }

        public RotateDirection Direction
        {
            get { return (RotateDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(RotateDirection),
            typeof(RotateContentControl), new PropertyMetadata(RotateDirection.Down, OnDirectionPropertyChanged));

        public static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((int)e.OldValue) % 180 == ((int)e.NewValue) % 180)
                (d as RotateContentControl).InvalidateArrange(); //flipping 180 degrees only changes flow not size
            else
                (d as RotateContentControl).InvalidateMeasure(); //flipping 90 or 270 degrees changes size too, so remeasure
        }
    }
}
