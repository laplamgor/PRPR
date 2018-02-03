using PRPR.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
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

        private async void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                RecycleAll();
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (UpdateActiveRange(ParentScrollViewer.VerticalOffset, ParentScrollViewer.ViewportHeight, DesiredSize.Width - Margin.Left - Margin.Right, true))
                {
                    RevirtualizeAll();
                }
            }
        }


        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(JustifiedWrapPanel), new PropertyMetadata(null, OnItemSourceChanged));

        public static async void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = (d as JustifiedWrapPanel);


            // Reassign the handler to new item set
            if (e.OldValue is INotifyCollectionChanged)
            {
                (e.OldValue as INotifyCollectionChanged).CollectionChanged -= p.ItemsSource_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged)
            {
                (e.NewValue as INotifyCollectionChanged).CollectionChanged += p.ItemsSource_CollectionChanged;
            }



            if (e.OldValue is IList itemsSource)
            {
                p.RecycleAll();
            }


            p.CheckParentUpdate();
            if (p.ParentScrollViewer != null)
            {
                p.UpdateActiveRange(p.ParentScrollViewer.VerticalOffset, p.ParentScrollViewer.ViewportHeight, p.DesiredSize.Width - p.Margin.Left - p.Margin.Right, true);
                Debug.WriteLine($"OnItemSourceChanged: New Range {p.FirstActive} ~ {p.LastActive}");
                p.RevirtualizeAll();

                // It is necessary to force an UI refresh before handling the incremental loading
                // The scrollable window size sticks with old item source
                p.UpdateLayout();

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
                p.UpdateActiveRange(p.ParentScrollViewer.VerticalOffset, p.ParentScrollViewer.ViewportHeight, p.DesiredSize.Width - p.Margin.Left - p.Margin.Right, true);
                p.RevirtualizeAll();
                //p.InvalidateMeasure();
                //p.InvalidateArrange();
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
            var top = (sender as ScrollViewer).HorizontalOffset;

            if (UpdateActiveRange((sender as ScrollViewer).VerticalOffset, (sender as ScrollViewer).ViewportHeight, e.NewSize.Width - this.Margin.Left - this.Margin.Right, true))
            {
                RevirtualizeAll();
            }
            await CheckNeedMoreItemAsync();
        }
        private async void ParentScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            // Update the active range
            var top = e.FinalView.HorizontalOffset;

            var scrollViewer = (sender as ScrollViewer);

            if (UpdateActiveRange(e.NextView.VerticalOffset, scrollViewer.ViewportHeight, this.DesiredSize.Width - this.Margin.Left - this.Margin.Right, false))
            {
                //Debug.WriteLine($"ParentScrollViewer_ViewChanging: New Range {FirstActive} ~ {LastActive}");
                RevirtualizeAll();
            }

            await CheckNeedMoreItemAsync();
        }



        private int FirstActive = -1;
        private int LastActive = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visibleTop"></param>
        /// <param name="visibleHeight"></param>
        /// <param name="parentWidth"></param>
        /// <param name="layoutChanged"></param>
        /// <param name="activeWindowScale"></param>
        /// <returns>Whether the range is updated</returns>
        bool UpdateActiveRange(double visibleTop, double visibleHeight, double parentWidth, bool layoutChanged, double activeWindowScale = 3)
        {
            var visibleCenter = visibleTop + visibleHeight / 2.0;
            var halfVisibleWindowsSize = (activeWindowScale / 2.0) * visibleHeight;
            var activeTop = visibleCenter - halfVisibleWindowsSize - RowHeight;
            var activeBottom = visibleCenter + halfVisibleWindowsSize;

            var oldFirst = FirstActive;
            var oldLast = LastActive;

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
                    else // The FirstActive is found and confirmed
                    {
                        if (!layoutChanged && oldFirst == FirstActive)
                        {
                            // If the layout has not changed and we found that the top is unchanged,
                            // We immedately conclude that bottom is not changed too
                            return false;
                        }
                    }

                    var childImage = (child as IImageWallItemImage);
                    var childWidth = ScaledWidth(childImage, RowHeight);


                   

                    bool newRow = position.X + childWidth > parentWidth && (position.Y != 0 || position.X != 0);
                    if (newRow)
                    {
                        // next row!
                        position.X = childWidth;
                        position.Y += RowHeight;
                        if (position.Y > activeBottom) // Cannot see this row within active window
                        {
                            // Last row is the last active
                            LastActive = (ItemsSource as IList).IndexOf(child) - 1;
                            return oldFirst != FirstActive || oldLast != LastActive;
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
            else
            {
                FirstActive = LastActive = - 1;
            }

            return oldFirst != FirstActive || oldLast != LastActive;
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            // Update the parent ScrollViewer
            CheckParentUpdate();

            
            double currentY = 0;
            
            List<UIElement> currentRow = new List<UIElement>();

            if (ItemsSource is IList items)
            {
                var maxRowWidth = availableSize.Width;

                var maxRowRatio = maxRowWidth / RowHeight;
                var currectRowRatio = 0.0;
                foreach (IImageWallItemImage item in items)
                {
                    bool newRow = (item.PreferredRatio + currectRowRatio > maxRowRatio) && (currentY != 0 || currectRowRatio != 0);
                    if (newRow)
                    {
                        // Process previous row
                        MeasureRow(currentRow, new Rect(0, currentY, maxRowWidth, RowHeight), false);
                        currentRow.Clear();

                        // Reset current row
                        currectRowRatio = 0;
                        currentY += RowHeight;
                    }

                    if (ContainerFromItem(item) != null) // Realized
                    {
                        // Arrange it
                        currentRow.Add(ContainerFromItem(item) as ContentControl);
                    }

                    // adjust the location for the next items
                    currectRowRatio += item.PreferredRatio;
                }

                // update value with the last line
                MeasureRow(currentRow, new Rect(0, currentY, availableSize.Width, RowHeight), true);
                currentY += RowHeight;
            }


            return new Size(availableSize.Width, currentY);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double rowWidth = 0;
            double currentY = 0;

            List<UIElement> currentRow = new List<UIElement>();

            if (ItemsSource is IList items && items.Count > 0)
            {
                IImageWallItemImage item;
                for (int i = 0; i <= Math.Min(items.Count - 1, LastActive); i++) // Dont care about items after last active item
                {
                    item = items[i] as IImageWallItemImage;
                    var itemWidth = ScaledWidth(item, RowHeight);


                    bool newRow = itemWidth + rowWidth > finalSize.Width && (currentY != 0 || rowWidth != 0);
                    if (newRow)
                    {
                        // Process previous row
                        ArrangeRow(currentRow, new Rect(0, currentY, finalSize.Width, RowHeight), false);
                        currentRow.Clear();

                        // Reset current row
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
                ArrangeRow(currentRow, new Rect(0, currentY, finalSize.Width, RowHeight), items.Count - 1 == LastActive);
            }
            CheckNeedMoreItemAsync();
            return finalSize;
        }

        private void MeasureRow(List<UIElement> items, Rect rowSpace, bool isLastRow)
        {
            // Calculate the scale factor
            var scaleX = isLastRow ? 1 : rowSpace.Width / (items.Sum(o => ((o as ContentControl).Content as IImageWallItemImage).PreferredRatio) * rowSpace.Height);

            double currentX = 0;
            var scaledHeight = rowSpace.Height * scaleX;
            foreach (var item in items)
            {
                // Place the item
                var itemWidth = ((item as ContentControl).Content as IImageWallItemImage).PreferredRatio * scaledHeight;
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


        public void ScrollIntoView(object item, ScrollIntoViewAlignment alignment)
        {
            if (ItemsSource is IList items)
            {
                int index = items.IndexOf(item);
                if (index != -1)
                {
                    // Get the top position of the given index

                    // Scroll the scrollviewer
                    switch (alignment)
                    {
                        default:
                        case ScrollIntoViewAlignment.Default:
                            double top = GetPositionY(index);

                            if (ParentScrollViewer.VerticalOffset + ParentScrollViewer.ViewportHeight + ParentScrollViewer.Margin.Top < top + RowHeight)
                            {
                                // The target is below the viewport, align the item to the button of viewport
                                ParentScrollViewer.ChangeView(null, top + RowHeight - ParentScrollViewer.ViewportHeight - ParentScrollViewer.Margin.Top, null, true);
                            }
                            else if (top < ParentScrollViewer.VerticalOffset)
                            {
                                // The target is above the viewport, align the item to the top of viewport
                                ParentScrollViewer.ChangeView(null, top, null, true);
                            }
                            break;
                        case ScrollIntoViewAlignment.Leading:
                            ParentScrollViewer.ChangeView(null, GetPositionY(index), null, true);
                            break;
                    }
                }
            }
        }
        
        private double GetPositionY(int index)
        {
            double currentY = 0;
            
            if (ItemsSource is IList items)
            {
                var maxRowWidth = this.DesiredSize.Width - this.Margin.Left - this.Margin.Right;

                var maxRowRatio = maxRowWidth / RowHeight;
                var currectRowRatio = 0.0;
                for (int i = 0; i <= index; i++)
                {
                    var item = items[i] as IImageWallItemImage;
                    bool newRow = (item.PreferredRatio + currectRowRatio > maxRowRatio) && (currentY != 0 || currectRowRatio != 0);
                    if (newRow)
                    {

                        // Reset current row
                        currectRowRatio = 0;
                        currentY += RowHeight;
                    }
                    

                    // adjust the location for the next items
                    currectRowRatio += item.PreferredRatio;
                }

                return currentY;
            }
            return 0;
        }


        public DependencyObject ContainerFromIndex (int index)
        {
            if (ItemsSource is IList items)
            {
                return ContainerFromItem(items[index]);
            }
            else
            {
                return null;
            }
        }
    }
}
