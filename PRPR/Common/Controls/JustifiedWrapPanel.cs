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

        public static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as JustifiedWrapPanel).InvalidateMeasure();
            (d as JustifiedWrapPanel).InvalidateArrange();
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

        public double RowHeight = 100;

        private ScrollViewer ParentScrollViewer = null;

        protected override Size MeasureOverride(Size availableSize)
        {
            // Update the parent ScrollViewer
            if (ParentScrollViewer != (this.Parent as ScrollViewer))
            {
                if (ParentScrollViewer != null)
                {
                    ParentScrollViewer.ViewChanging -= ParentScrollViewer_ViewChanging;
                }
                ParentScrollViewer = (this.Parent as ScrollViewer);
                if (ParentScrollViewer != null)
                {
                    ParentScrollViewer.ViewChanging += ParentScrollViewer_ViewChanging;
                }
            }
            


            var totalMeasure = new UvMeasure();
            var parentMeasure = new UvMeasure(availableSize.Width, availableSize.Height);

            double rowWidth = 0;

            var currentRowIndex = 0;

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
                    // Estimate the child size
                    var itemWidth = ScaledWidth(item, RowHeight);

                    if (ContainerFromItem(item) != null) // Realized
                    {
                        // Measure it
                        (ContainerFromItem(item) as ContentControl).Measure(new Size(ScaledWidth(item, RowHeight), RowHeight));
                    }

                    bool newRow = itemWidth + rowWidth > parentMeasure.X;
                    if (!newRow)
                    {
                        rowWidth += itemWidth;
                    }
                    else
                    {
                        // new line should be added
                        // to get the max U to provide it correctly to ui width ex: ---| or -----|
                        totalMeasure.Y += RowHeight;

                        // if the next new row still can handle more controls
                        if (parentMeasure.X > itemWidth)
                        {
                            // set lineMeasure initial values to the currentMeasure to be calculated later on the new loop
                            rowWidth = itemWidth;
                        }
                        // the control will take one row alone
                        else
                        {
                            // validate the new control measures
                            totalMeasure.Y += RowHeight;

                            // add new empty line
                            rowWidth = 0;
                        }
                    }
                }
            }

            // update value with the last line
            totalMeasure.X = parentMeasure.X;
            totalMeasure.Y += RowHeight;

            return new Size(totalMeasure.X, totalMeasure.Y);
        }


        private int FirstActive = -1;
        private int LastActive = -1;
        
        void UpdateActiveRange(double visibleTop, double visibleHeight, double parentWidth, double activeWindowScale = 3)
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
                        FirstActive = (ItemsSource as IList).IndexOf(child) + 1;
                    }

                    if (position.Y < activeTop)
                    {
                        LastActive = (ItemsSource as IList).IndexOf(child) - 1;
                    }

                    var childWidth = (child as IImageWallItemImage).PreferredWidth / (child as IImageWallItemImage).PreferredHeight * RowHeight;


                    bool newRow = childWidth + position.X > parentWidth;
                    if (newRow)
                    {

                        // next row!
                        position.X = 0;
                        position.Y += RowHeight;
                        if (position.Y > activeBottom) // Cannot see this row within active window
                        {
                            // Last row is the last active
                            LastActive = (ItemsSource as IList).IndexOf(child) - 1;
                            return;
                        }
                    }
                   

                    // adjust the location for the next items
                    position.X += childWidth;
                }

                LastActive = (ItemsSource as IList).Count - 1;
            }
        }

        private void ParentScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            // Update the active range
            // TODO: handle oriendation
            var top = e.FinalView.HorizontalOffset;


            UpdateActiveRange(e.FinalView.VerticalOffset, (sender as ScrollViewer).ViewportHeight, (sender as ScrollViewer).DesiredSize.Width, 1);
            Debug.WriteLine($"Range = {FirstActive}-{LastActive}");


            // TODO: select the range of active items to be realized
            InvalidateMeasure();
            InvalidateArrange();
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            var parentMeasure = new UvMeasure(finalSize.Width, finalSize.Height);
            var position = new UvMeasure();

            List<UIElement> currentRow = new List<UIElement>();
            // Realize all items

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


                    bool newRow = itemWidth + position.X > parentMeasure.X;
                    if (newRow)
                    {
                        // Process previous row
                        ArrangeRow(currentRow, new Rect(0, position.Y, finalSize.Width, RowHeight), false);
                        currentRow.Clear();

                        // next row!
                        position.X = 0;
                        position.Y += RowHeight;
                    }

                    if (ContainerFromItem(item) != null) // Realized
                    {
                        // Arrange it
                        currentRow.Add(ContainerFromItem(item) as ContentControl);
                    }

                    // adjust the location for the next items
                    position.X += itemWidth;
                }
            }

            ArrangeRow(currentRow, new Rect(0, position.Y, finalSize.Width, RowHeight), true);

            return finalSize;
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
