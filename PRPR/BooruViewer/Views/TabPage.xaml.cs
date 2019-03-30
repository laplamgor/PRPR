using Microsoft.Toolkit.Uwp.UI.Controls;
using PRPR.BooruViewer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PRPR.BooruViewer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabPage : Page
    {
        public TabPage()
        {
            this.InitializeComponent();
        }

        SpriteVisual spriteVisual = null;
        Rectangle tabListBackground = null;
        TabViewItem selectedItem = null;
        ScrollViewer scrollViewer = null;
        ScrollContentPresenter scrollContentPresenter = null;

        private void Tabs_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TabView).Items.Add(CreateTabViewItem(new TabSummary()));
            (sender as TabView).Items.Add(CreateTabViewItem(new TabSummary()));
            (sender as TabView).Items.Add(CreateTabViewItem(new TabSummary()));
            (sender as TabView).Items.Add(CreateTabViewItem(new TabSummary()));


            // Find the acrylic rect
            var grid = VisualTreeHelper.GetChild(Tabs, 0) as Grid;
            tabListBackground = grid.Children.FirstOrDefault(o => (o as Rectangle)?.Tag as string == "Acrylic") as Rectangle;


            //// Find the scrollviewer
            scrollViewer = grid.Children.FirstOrDefault(o => (o as ScrollViewer)?.Tag as string == "ScrollViewer") as ScrollViewer;
            scrollViewer.SizeChanged += Scrollviewer_SizeChanged;
            scrollViewer.ViewChanging += Scrollviewer_ViewChanging;


            var g = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(scrollViewer, 0) as Border, 0) as Grid;
            scrollContentPresenter = g.Children.FirstOrDefault(o => o is ScrollContentPresenter) as ScrollContentPresenter;

            //var repeatPrevButton = g.Children.FirstOrDefault(o => o is RepeatButton) as RepeatButton;
            //repeatPrevButton.SizeChanged += RepeatPrevButton_SizeChanged;
            scrollContentPresenter.SizeChanged += ScrollContentPresenter_SizeChanged;

            {
                var _compositor = ElementCompositionPreview.GetElementVisual(tabListBackground).Compositor;
                spriteVisual = _compositor.CreateSpriteVisual();
                spriteVisual.Brush = _compositor.CreateColorBrush(Colors.Transparent);
                var shadow = _compositor.CreateDropShadow();
                shadow.BlurRadius = 20;
                shadow.Opacity = 0.5f;
                shadow.Color = (Color)Application.Current.Resources["SystemBaseMediumHighColor"];
                spriteVisual.Shadow = shadow;
                ElementCompositionPreview.SetElementChildVisual(tabListBackground, spriteVisual);
            }



            if (scrollViewer.ScrollableWidth >= 65)
            {
                UpdateShadowClip(true);
            }
            else
            {
                UpdateShadowClip(false);
            }
        }




        private TabViewItem CreateTabViewItem(Tab content)
        {
            ContentControl container = new ContentControl();
            {
                container.Content = content;
                container.Style = Tabs.ItemContainerStyle;
                container.ContentTemplate = (DataTemplate)this.Resources[content.GetType().Name]; ;
            }


            var item = new TabViewItem()
            {
                Content = container
            };

            if (content is TabSummary)
            {
                item.Header = "Home";
                item.Icon = new SymbolIcon() { Symbol = Symbol.Home };
                item.IsClosable = false;
            } else if (content is TabPostList)
            {
                item.Header = "Search";
                item.Icon = new SymbolIcon() { Symbol = Symbol.Find };
            }
            else if (content is TabPostDetail)
            {
                item.Header = "Image";
                item.Icon = new SymbolIcon() { Symbol = Symbol.BrowsePhotos };
            }


            return item;
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                if (selectedItem != null)
                {
                    selectedItem.SizeChanged -= TabViewItem_SizeChanged;
                }
                selectedItem = null;
            }

            foreach (var item in e.AddedItems)
            {
                if (selectedItem != null)
                {
                    selectedItem.SizeChanged -= TabViewItem_SizeChanged;
                }

                selectedItem = item as TabViewItem;
                UpdateShadowSizeAndPosition();
                (item as TabViewItem).SizeChanged += TabViewItem_SizeChanged;
            }
        }

        private void TabViewItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateShadowSizeAndPosition();
        }

        private void UpdateShadowSizeAndPosition(float x = 0)
        {
            if (selectedItem != null)
            {
                var point = (selectedItem).TransformToVisual(tabListBackground).TransformPoint(new Point(0, 0));

                // Trim the width occluded by the add tab button
                float width = (float)selectedItem.ActualWidth;
                var actualTabEndX = width + point.X;
                var addButtonStartX = (float)(scrollViewer).TransformToVisual(tabListBackground).TransformPoint(new Point(0, 0)).X + scrollViewer.ActualWidth;

                spriteVisual.Size = new System.Numerics.Vector2((float)(Math.Min(actualTabEndX, addButtonStartX) - point.X), (float)selectedItem.ActualHeight);
                (spriteVisual.Shadow as DropShadow).Offset = new System.Numerics.Vector3((float)point.X + x, (float)point.Y + 8, 0);

            }
        }

        private void UpdateShadowClip(bool cropForButtons)
        {
            if (cropForButtons)
            {
                var visual2 = ElementCompositionPreview.GetElementVisual(tabListBackground);
                visual2.Clip = ElementCompositionPreview.GetElementVisual(tabListBackground).Compositor.CreateInsetClip(48+32,0,32+48+48,0);
            }
            else
            {
                var visual2 = ElementCompositionPreview.GetElementVisual(tabListBackground);
                visual2.Clip = ElementCompositionPreview.GetElementVisual(tabListBackground).Compositor.CreateInsetClip(0, 0, 0, 0);
            }
        }

        private void AddTabButtonUpper_Click(object sender, RoutedEventArgs e)
        {
            UpdateShadowClip(true);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateShadowClip(false);
        }

        private void Tabs_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            spriteVisual.IsVisible = false;
        }

        private void Tabs_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            spriteVisual.IsVisible = true;
        }

        private void ScrollContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateShadowSizeAndPosition();
        }

        private void Scrollviewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            UpdateShadowSizeAndPosition();
        }

        private void Scrollviewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((sender as ScrollViewer).ScrollableWidth >= 65)
            {
                UpdateShadowClip(true);
            }
            else
            {
                UpdateShadowClip(false);
            }

            UpdateShadowSizeAndPosition();
        }
    }

}
