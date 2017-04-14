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
    public class JustifiedWrapPanel : Panel
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
            var rowMeasure = new UvMeasure();

            var currentRowIndex = 0;
            foreach (var child in Children)
            {
                // Estimate the child size
                // TODO: Do not use Measure() because we need virtualization
                child.Measure(availableSize);
                var childMeasure = new UvMeasure(child.DesiredSize.Width, child.DesiredSize.Height);

                bool newRow = childMeasure.X + rowMeasure.X > parentMeasure.X;
                if (!newRow)
                {
                    rowMeasure.X += childMeasure.X;
                }
                else
                {
                    // new line should be added
                    // to get the max U to provide it correctly to ui width ex: ---| or -----|
                    totalMeasure.X = parentMeasure.X;
                    totalMeasure.Y += RowHeight;

                    // if the next new row still can handle more controls
                    if (parentMeasure.X > childMeasure.X)
                    {
                        // set lineMeasure initial values to the currentMeasure to be calculated later on the new loop
                        rowMeasure = childMeasure;
                    }
                    // the control will take one row alone
                    else
                    {
                        // validate the new control measures
                        totalMeasure.X = parentMeasure.X;
                        totalMeasure.Y += childMeasure.Y;

                        // add new empty line
                        rowMeasure = new UvMeasure();
                    }
                }
            }

            // update value with the last line
            totalMeasure.X = parentMeasure.X;
            totalMeasure.Y += RowHeight;

            return new Size(totalMeasure.X, totalMeasure.Y);
        }

        private void ParentScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            // TODO: handle oriendation
            var top = e.FinalView.HorizontalOffset;


            // TODO: select the range of active items to be realized
            InvalidateMeasure();
            InvalidateArrange();
        }
        

        protected override Size ArrangeOverride(Size finalSize)
        {
            var parentMeasure = new UvMeasure(finalSize.Width, finalSize.Height);
            var position = new UvMeasure();
            
            List<UIElement> currentRow = new List<UIElement>();
            foreach (var child in Children)
            {
                var childMeasure = new UvMeasure(child.DesiredSize.Width, child.DesiredSize.Height);

                bool newRow = childMeasure.X + position.X > parentMeasure.X;
                if (newRow)
                {
                    // Process previous row
                    ArrangeRow(currentRow, new Rect(0, position.Y, finalSize.Width, RowHeight), false);
                    currentRow.Clear();

                    // next row!
                    position.X = 0;
                    position.Y += RowHeight;
                }

                currentRow.Add(child);

                // adjust the location for the next items
                position.X += childMeasure.X;
            }

            ArrangeRow(currentRow, new Rect(0, position.Y, finalSize.Width, RowHeight), true);

            return finalSize;
        }

        private void ArrangeRow(List<UIElement> items, Rect rowSpace, bool isLastRow)
        {
            // Calculate the scale factor
            var scaleX = isLastRow ? 1 : rowSpace.Width / items.Sum(o => o.DesiredSize.Width);

            double currentX = 0;
            foreach (var item in items)
            {
                // Place the item
                item.Arrange(new Rect(rowSpace.X + currentX, rowSpace.Y, 
                    item.DesiredSize.Width * scaleX, 
                    rowSpace.Height));
                currentX += item.DesiredSize.Width * scaleX;
            }
        }
    }
}
