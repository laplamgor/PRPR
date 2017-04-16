using PRPR.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PRPR.Common.Controls
{
    public partial class JustifiedWrapPanel : Panel
    {
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(JustifiedWrapPanel), new PropertyMetadata(null, OnItemSourceChanged));

        public static async void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = (d as JustifiedWrapPanel);
            p.CheckParentUpdate();
            if (p.ParentScrollViewer != null)
            {
                p.UpdateActiveRange(p.ParentScrollViewer.VerticalOffset, p.ParentScrollViewer.ViewportHeight, p.DesiredSize.Width - p.Margin.Left - p.Margin.Right);
                p.InvalidateMeasure();
                p.InvalidateArrange();
                await p.CheckNeedMoreItemAsync();
            }
        }

        
        


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(JustifiedWrapPanel), new PropertyMetadata(null));




        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(JustifiedWrapPanel), new PropertyMetadata(null));




        
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(JustifiedWrapPanel), new PropertyMetadata(100.0, OnRowHeightChanged));

        public static async void OnRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = (d as JustifiedWrapPanel);
            p.CheckParentUpdate();
            if (p.ParentScrollViewer != null)
            {
                p.UpdateActiveRange(p.ParentScrollViewer.VerticalOffset, p.ParentScrollViewer.ViewportHeight, p.DesiredSize.Width - p.Margin.Left - p.Margin.Right);
                p.InvalidateMeasure();
                p.InvalidateArrange();
                await p.CheckNeedMoreItemAsync();
            }
        }









        internal class UvMeasure
        {
            internal double X { get; set; }

            internal double Y { get; set; }

            public UvMeasure()
            {
                X = 0.0;
                Y = 0.0;
            }

            public UvMeasure(double width, double height)
            {
                X = width;
                Y = height;
            }
        }


        public ScrollViewer ParentScrollViewer = null;

        private void CheckParentUpdate()
        {
            if (ParentScrollViewer != (Parent as ScrollViewer))
            {
                if (ParentScrollViewer != null)
                {
                    ParentScrollViewer.ViewChanging -= ParentScrollViewer_ViewChanging;
                    ParentScrollViewer.SizeChanged -= ParentScrollViewer_SizeChanged;
                }
                ParentScrollViewer = (this.Parent as ScrollViewer);
                if (ParentScrollViewer != null)
                {
                    ParentScrollViewer.ViewChanging += ParentScrollViewer_ViewChanging;
                    ParentScrollViewer.SizeChanged += ParentScrollViewer_SizeChanged;
                }
            }
        }

        private async void ParentScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO: handle oriendation
            var top = (sender as ScrollViewer).HorizontalOffset;

            UpdateActiveRange((sender as ScrollViewer).VerticalOffset, (sender as ScrollViewer).ViewportHeight, e.NewSize.Width - this.Margin.Left - this.Margin.Right);
            InvalidateMeasure();
            InvalidateArrange();
            await CheckNeedMoreItemAsync();
        }

        private int FirstActive = -1;
        private int LastActive = -1;
        
        void UpdateActiveRange(double visibleTop, double visibleHeight, double parentWidth, double activeWindowScale = 6)
        {
            var visibleCenter = visibleTop + visibleHeight / 2.0;
            var halfVisibleWindowsSize = (activeWindowScale / 2.0) * visibleHeight;
            var activeTop = visibleCenter - halfVisibleWindowsSize - RowHeight;
            var activeBottom = visibleCenter + halfVisibleWindowsSize;


            FirstActive = -1;
            if (ItemsSource is IList)
            {
                if ((ItemsSource as IList).Count != 0)
                {
                    FirstActive = 0;
                }
                var position = new UvMeasure();
                foreach (var child in ItemsSource as IList)
                {
                    if (position.Y < activeTop) // Cannot see this row within active window
                    {
                        FirstActive = (ItemsSource as IList).IndexOf(child);
                    }

                    var childImage = (child as IImageWallItemImage);
                    var childWidth = ScaledWidth(childImage, RowHeight);


                   

                    bool newRow = position.X + childWidth > parentWidth;
                    if (newRow)
                    {
                        // next row!
                        position.X = childWidth;
                        position.Y += RowHeight;
                        if (position.Y > activeBottom) // Cannot see this row within active window
                        {
                            // Last row is the last active
                            LastActive = (ItemsSource as IList).IndexOf(child) - 1;
                            return;
                        }
                    }
                    else
                    {
                        // adjust the location for the next items
                        position.X += childWidth;
                    }
                }

                LastActive = (ItemsSource as IList).Count - 1;
            }
        }

        private async void ParentScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            // Update the active range
            // TODO: handle oriendation
            var top = e.FinalView.HorizontalOffset;

            var scrollViewer = (sender as ScrollViewer);

            UpdateActiveRange(e.NextView.VerticalOffset, scrollViewer.ViewportHeight, this.DesiredSize.Width - this.Margin.Left - this.Margin.Right);

            Debug.WriteLine($"New Range {FirstActive} ~ {LastActive}");
            InvalidateMeasure();
            InvalidateArrange();
            await CheckNeedMoreItemAsync();
        }

        

        protected override Size MeasureOverride(Size availableSize)
        {
            // Update the parent ScrollViewer
            CheckParentUpdate();


            double rowWidth = 0;
            double currentY = 0;
            
            List<UIElement> currentRow = new List<UIElement>();

            if (ItemsSource is IList items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i >= FirstActive && i <= LastActive)
                    {
                        RealizeItem(items[i]);
                    }
                    else
                    {
                        RecycleItem(items[i]);
                    }
                }
                

                foreach (IImageWallItemImage item in items)
                {
                    var itemWidth = ScaledWidth(item, RowHeight);


                    bool newRow = itemWidth + rowWidth > availableSize.Width;
                    if (newRow)
                    {
                        // Process previous row
                        MeasureRow(currentRow, new Rect(0, currentY, availableSize.Width, RowHeight), false);
                        currentRow.Clear();

                        // next row!
                        rowWidth = 0;
                        currentY += RowHeight;
                    }

                    if (ContainerFromItem(item) != null) // Realized
                    {
                        // Arrange it
                        currentRow.Add(ContainerFromItem(item) as ContentControl);
                    }

                    // adjust the location for the next items
                    rowWidth += itemWidth;
                }
            }


            MeasureRow(currentRow, new Rect(0, currentY, availableSize.Width, RowHeight), true);

            // update value with the last line
            currentY += RowHeight;
            return new Size(availableSize.Width, currentY);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            double rowWidth = 0;
            double currentY = 0;

            List<UIElement> currentRow = new List<UIElement>();

            if (ItemsSource is IList items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i >= FirstActive && i <= LastActive)
                    {
                        RealizeItem(items[i]);
                    }
                    else
                    {
                        RecycleItem(items[i]);
                    }
                }

                foreach (IImageWallItemImage item in items)
                {
                    var itemWidth = ScaledWidth(item, RowHeight);


                    bool newRow = itemWidth + rowWidth > finalSize.Width;
                    if (newRow)
                    {
                        // Process previous row
                        ArrangeRow(currentRow, new Rect(0, currentY, finalSize.Width, RowHeight), false);
                        currentRow.Clear();

                        // next row!
                        rowWidth = 0;
                        currentY += RowHeight;
                    }

                    if (ContainerFromItem(item) != null) // Realized
                    {
                        // Arrange it
                        currentRow.Add(ContainerFromItem(item) as ContentControl);
                    }

                    // adjust the location for the next items
                    rowWidth += itemWidth;
                }
            }

            ArrangeRow(currentRow, new Rect(0, currentY, finalSize.Width, RowHeight), true);

            return finalSize;
        }

        private void MeasureRow(List<UIElement> items, Rect rowSpace, bool isLastRow)
        {
            // Calculate the scale factor
            var scaleX = isLastRow ? 1 : rowSpace.Width / items.Sum(o =>
           ScaledWidth((o as ContentControl).Content as IImageWallItemImage, rowSpace.Height));

            double currentX = 0;
            foreach (var item in items)
            {
                // Place the item
                var itemWidth = ScaledWidth((item as ContentControl).Content as IImageWallItemImage, rowSpace.Height) * scaleX;
                var itemSize = new Size(itemWidth, rowSpace.Height);
                item.Measure(itemSize);
                currentX += itemWidth;
            }
        }


        private void ArrangeRow(List<UIElement> items, Rect rowSpace, bool isLastRow)
        {
            // Calculate the scale factor
            var scaleX = isLastRow ? 1 : rowSpace.Width / items.Sum( o =>
            ScaledWidth((o as ContentControl).Content as IImageWallItemImage, rowSpace.Height));

            double currentX = 0;
            foreach (var item in items)
            {
                // Place the item
                var itemWidth = ScaledWidth((item as ContentControl).Content as IImageWallItemImage, rowSpace.Height) * scaleX;
                var itemRect = new Rect(rowSpace.X + currentX, rowSpace.Y, itemWidth, rowSpace.Height);
                item.Arrange(itemRect);
                currentX += itemWidth;
            }
        }


        double ScaledWidth(IImageWallItemImage item, double height)
        {
            return item.PreferredWidth / item.PreferredHeight * height;
        }
    }
}
