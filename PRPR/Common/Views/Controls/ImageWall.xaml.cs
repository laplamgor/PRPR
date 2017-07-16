using PRPR.BooruViewer.Models;
using PRPR.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PRPR.Common.Views.Controls
{
    public sealed partial class ImageWall : UserControl
    {
        public ImageWall()
        {
            this.InitializeComponent();

            this.DataContext = null;
        }


        private double getElementPositionY(UIElement element)
        {
            var trans = element.TransformToVisual(null);
            var point = trans.TransformPoint(new Point());
            return point.Y;
        }
        
        private int getListViewCurrentRowIndex()
        {
            var listViewY = getElementPositionY(RowsListView);


            var minOffset = double.MaxValue;
            int minOffsetPageIndex = -1;

            // Return -1 if there is no content loaded in the listview
            if (RowsListView.Items.Count == 0)
            {
                return -1;
            }


            for (int i = 0; i < RowsListView.Items.Count(); i++)
            {
                var container = RowsListView.ContainerFromIndex(i);
                if (container != null)
                {
                    if (((FrameworkElement)container).ActualHeight != 0)
                    {
                        var y = getElementPositionY((UIElement)container) - listViewY;
                        if (y != -listViewY)
                        {
                            // If the item is virtualized, its position Y will be 0
                            if (Math.Abs(y) <= minOffset)
                            {
                                minOffset = Math.Abs(y);
                                minOffsetPageIndex = i;
                            }
                        }
                    }
                }
                else
                {
                    // This container is virtualized, Do nothing
                }
            }
            return minOffsetPageIndex;
        }





        public DataTemplate ItemTemplate
        {
            get
            {
                var t = (DataTemplate)GetValue(ItemTemplateProperty);
                return t;
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }


        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ImageWall), new PropertyMetadata(null));





        public object ContainerFromItem(object item)
        {
            if (RowsListView.ItemsSource is ImageWallRows<Post> rows)
            {
                foreach (var row in rows)
                {
                    if (RowsListView.ContainerFromItem(row) != null)
                    {


                        var container = RowsListView.ContainerFromItem(row) as ListViewItem;
                        if (container != null)
                        {
                            var root = (FrameworkElement)container.ContentTemplateRoot;
                            var innerListView = (ListView)root.FindName("InnerListView");

                            var c = innerListView.ContainerFromItem(item);
                            if (c != null)
                            {
                                return c;
                            }
                        }
                    }
                }
            }
            

            return null;
        }



        public event ItemClickEventHandler ItemClick;

        private async void RowsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.DataContext == null)
            {
                return;
            }

            if ((RowsListView.ItemsSource as IImageWallRows) != null)
            {
                if (!resizePending && e.NewSize.Width - this.Padding.Left - this.Padding.Right == (RowsListView.ItemsSource as IImageWallRows).RowWidth)
                {
                    return;
                }

                if (resizePending)
                {
                    resizePending = false;


                    var temp = e.NewSize;

                    await Task.Delay(500);

                    if (RowsListView.RenderSize.Width == temp.Width)
                    {
                        var currentRow = getListViewCurrentRowIndex();
                        int currentRowMidItemIndex = 0;
                        if (currentRow != -1)
                        {
                            currentRowMidItemIndex = (RowsListView.ItemsSource as IEnumerable<object>).Take(currentRow).Sum(o => (o as IEnumerable<object>).Count());
                            currentRowMidItemIndex += ((dynamic)RowsListView.ItemsSource)[currentRow].Count / 2;
                        }


                        (RowsListView.ItemsSource as IImageWallRows).RowWidth = RowsListView.ActualWidth - RowsListView.Padding.Left - RowsListView.Padding.Right;
                        (RowsListView.ItemsSource as IImageWallRows).RowHeight = RowsListView.ActualWidth > 500 ? 300 : 150;
                        (RowsListView.ItemsSource as IImageWallRows).Resize();

                        if (currentRow != -1)
                        {
                            int sum = 0;
                            foreach (var row in (RowsListView.ItemsSource as IEnumerable<object>))
                            {
                                sum += (row as IEnumerable<object>).Count();
                                if (sum >= currentRowMidItemIndex)
                                {
                                    RowsListView.ScrollIntoView(row, ScrollIntoViewAlignment.Leading);
                                    break;
                                }
                            }

                        }
                    }

                }
                else
                {

                    var temp = e.NewSize;

                    await Task.Delay(500);

                    if (RowsListView.RenderSize.Width == temp.Width)
                    {
                        var currentRow = getListViewCurrentRowIndex();
                        int currentRowMidItemIndex = 0;
                        if (currentRow != -1)
                        {
                            currentRowMidItemIndex = (RowsListView.ItemsSource as IEnumerable<object>).Take(currentRow).Sum(o => (o as IEnumerable<object>).Count());
                            if (currentRow + 1 <= (RowsListView.ItemsSource as IList).Count)
                            {
                                var next = (RowsListView.ItemsSource as IEnumerable<object>).Take(currentRow + 1).Sum(o => (o as IEnumerable<object>).Count());
                                currentRowMidItemIndex = (currentRowMidItemIndex + next) / 2;
                            }
                        }


                        (RowsListView.ItemsSource as IImageWallRows).RowWidth = RowsListView.ActualWidth - RowsListView.Padding.Left - RowsListView.Padding.Right;
                        (RowsListView.ItemsSource as IImageWallRows).RowHeight = RowsListView.ActualWidth > 500 ? 300 : 150;
                        (RowsListView.ItemsSource as IImageWallRows).Resize();

                        if (currentRow != -1)
                        {
                            int sum = 0;
                            foreach (var row in (RowsListView.ItemsSource as IEnumerable<object>))
                            {
                                sum += (row as IEnumerable<object>).Count();
                                if (sum >= currentRowMidItemIndex)
                                {
                                    RowsListView.ScrollIntoView(row, ScrollIntoViewAlignment.Leading);
                                    break;
                                }
                            }

                        }
                    }
                }




            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //bubble the event up to the parent
            this.ItemClick?.Invoke(this, e);
        }

        bool resizePending = false;
        private void UserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // Raise when newly created control
            if (args.NewValue == null)
            {
                return;
            }



            // Change the RowHeight and RowWidth
            if (RowsListView.ItemsSource != null && this.DataContext is IImageWallRows && this.ActualWidth != 0)
            {
                (this.DataContext as IImageWallRows).RowWidth = this.ActualWidth - this.Padding.Left - this.Padding.Right;
                (this.DataContext as IImageWallRows).RowHeight = this.ActualWidth > 500 ? 300 : 150;
                (RowsListView.ItemsSource as IImageWallRows).Resize();
            }
            else if (RowsListView.ItemsSource != null && (this.DataContext as IList).Count == 0)
            {
                resizePending = true;
            }


        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            var i = sender as Image;

            var b = i.Parent as Border;
            if (b == null)
            {
                i.Opacity = 1;
                return;
            }

            var g = b.Parent as Grid;
            if (g == null)
            {
                i.Opacity = 1;
                return;
            }


            var c = g.Parent as UserControl;
            if (c == null)
            {
                i.Opacity = 1;
                return;
            }

            VisualStateManager.GoToState(c, "ImageLoaded", true);
        }

        private void Wall_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.RenderSize.Width >= 1280)
            {
                var b = VisualStateManager.GoToState(this, "Padding1280", false);
            }
            else if (this.RenderSize.Width >= 600)
            {
                var b = VisualStateManager.GoToState(this, "Padding600", false);
            }
            else if (this.RenderSize.Width >= 400)
            {
                var b = VisualStateManager.GoToState(this, "Padding400", false);
            }
            else
            {
                var b = VisualStateManager.GoToState(this, "Padding0", false);
            }
            Wall.InvalidateMeasure();
            Wall.InvalidateArrange();
            Wall.UpdateLayout();

            // Change the RowHeight and RowWidth
            if (RowsListView.ItemsSource != null && this.DataContext != null)
            {
                var isSourceChanged = (RowsListView.ItemsSource as IImageWallRows).GetSource() == this.DataContext;
                if (isSourceChanged)
                {
                    (this.DataContext as IImageWallRows).RowWidth = this.ActualWidth - this.Padding.Left - this.Padding.Right;
                    (this.DataContext as IImageWallRows).RowHeight = this.ActualWidth > 500 ? 300 : 150;
                    (RowsListView.ItemsSource as IImageWallRows).Resize();
                }
            }
            else
            {

                if (this.DataContext != null && this.DataContext is IImageWallRows && this.ActualWidth != 0)
                {
                    (this.DataContext as IImageWallRows).RowWidth = this.ActualWidth - this.Padding.Left - this.Padding.Right;
                    (this.DataContext as IImageWallRows).RowHeight = this.ActualWidth > 500 ? 300 : 150;
                    (RowsListView.ItemsSource as IImageWallRows).Resize();
                }
                else if (RowsListView.ItemsSource != null && (this.DataContext as IList).Count == 0)
                {
                    resizePending = true;
                }
            }


        }
    }
    
}
