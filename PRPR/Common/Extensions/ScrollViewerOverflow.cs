using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace PRPR.Common.Extensions
{
    public static class ScrollViewerOverflow
    {
        public static readonly DependencyProperty OverflowMarginProperty = 
            DependencyProperty.RegisterAttached("OverflowMargin", typeof(Thickness), typeof(ScrollViewerOverflow), new PropertyMetadata(null, OnOverflowMarginPropertyChanged));

        private static void OnOverflowMarginPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement baseElement))
            {
                return;
            }

            // If it didn't work it means that we need to wait for the component to be loaded before getting its ScrollViewer
            if (ChangeOverflowMarginProperty(sender as FrameworkElement))
            {
                return;
            }

            // We need to wait for the component to be loaded before getting its ScrollViewer
            baseElement.Loaded -= ChangeOverflowMarginProperty;

            if (OverflowMarginProperty != null)
            {
                baseElement.Loaded += ChangeOverflowMarginProperty;
            }
        }

        
        public static void SetOverflowMargin(FrameworkElement element, Thickness value)
        {
            element.SetValue(OverflowMarginProperty, value);

            if (element is ScrollViewer)
            {
                element.GettingFocus += Element_GettingFocus;
                element.GotFocus += Element_GotFocus;
            }
        }

        private static void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            (FocusManager.GetFocusedElement() as DependencyObject).ClearValue(FrameworkElement.FocusVisualMarginProperty);
            (FocusManager.GetFocusedElement() as DependencyObject).ClearValue(FrameworkElement.FocusVisualPrimaryThicknessProperty);
            (FocusManager.GetFocusedElement() as DependencyObject).ClearValue(FrameworkElement.FocusVisualSecondaryThicknessProperty);
        }

        private static void Element_GettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
            args.NewFocusedElement.SetValue(FrameworkElement.FocusVisualMarginProperty, new Thickness(0, -150, 0, 0));
            args.NewFocusedElement.SetValue(FrameworkElement.FocusVisualPrimaryThicknessProperty, new Thickness(0, 0, 0, 0));
            args.NewFocusedElement.SetValue(FrameworkElement.FocusVisualSecondaryThicknessProperty, new Thickness(0, 0, 0, 0));
        }

        public static Thickness GetOverflowMargin(FrameworkElement element)
        {
            return (Thickness)element.GetValue(OverflowMarginProperty);
        }




        private static bool ChangeOverflowMarginProperty(FrameworkElement sender)
        {
            if (sender == null)
            {
                return false;
            }

            var scrollViewer = sender as ScrollViewer ?? sender.FindDescendant<ScrollViewer>();
            
            var scrollBar = scrollViewer?.FindDescendants<ScrollBar>().LastOrDefault(bar => bar.Name == "VerticalScrollBar");
            if (scrollBar != null)
            {
                var newMargin = GetOverflowMargin(sender);
                //scrollBar.Margin = newMargin;
            }


            var scrollContent = scrollViewer?.Content as FrameworkElement;
            if (scrollContent != null)
            {
                var newMargin = GetOverflowMargin(sender);
                scrollContent.Margin = new Thickness(
                    scrollContent.Margin.Left + newMargin.Left, 
                    scrollContent.Margin.Top + newMargin.Top,
                    scrollContent.Margin.Right + newMargin.Right,
                    scrollContent.Margin.Bottom + newMargin.Bottom);
            }

            var scrollContentPresenter = scrollViewer?.FindDescendants<ScrollContentPresenter>().FirstOrDefault(bar => bar.Name == "ScrollContentPresenter");
            if (scrollContentPresenter != null)
            {
                var newMargin = GetOverflowMargin(sender);
                scrollContentPresenter.Margin = new Thickness(newMargin.Left, -newMargin.Top, newMargin.Right, newMargin.Bottom);
                return true;
            }


            return false;
        }


        private static void ChangeOverflowMarginProperty(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is FrameworkElement baseElement))
            {
                return;
            }

            ChangeOverflowMarginProperty(baseElement);

            // Handling Loaded event is only required the first time the property is set, so we can stop handling it now
            baseElement.Loaded -= ChangeOverflowMarginProperty;
        }
    }
}
