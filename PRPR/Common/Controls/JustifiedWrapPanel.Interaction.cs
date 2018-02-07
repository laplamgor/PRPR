using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace PRPR.Common.Controls
{
    public partial class JustifiedWrapPanel
    {

        private void Container_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter:
                case Windows.System.VirtualKey.Accept:
                case Windows.System.VirtualKey.Space:
                case Windows.System.VirtualKey.Select:
                    if (this.ItemClick != null)
                    {
                        this.ItemClick(this, new ItemClickEventArgs((sender as ContentControl).Content));
                        e.Handled = true;
                    }
                    break;
            }
        }

        public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);

        public event ItemClickEventHandler ItemClick;

        public sealed class ItemClickEventArgs
        {
            public object ClickedItem
            {
                get;
                private set;
            }

            internal ItemClickEventArgs(object clickedItem)
            {
                ClickedItem = clickedItem;
            }
        }

        private void Container_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (this.ItemClick != null)
            {
                this.ItemClick(this, new ItemClickEventArgs((sender as ContentControl).Content));
                e.Handled = true;
            }
        }


        private void JustifiedWrapPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            DependencyObject candidate = null;

            var options = new FindNextElementOptions()
            {
                SearchRoot = this,
                XYFocusNavigationStrategyOverride = XYFocusNavigationStrategyOverride.Projection
            };
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Up:
                    candidate = FocusManager.FindNextElement(FocusNavigationDirection.Up, options);
                    break;
                case Windows.System.VirtualKey.Down:
                    candidate = FocusManager.FindNextElement(FocusNavigationDirection.Down, options);
                    break;
                case Windows.System.VirtualKey.Left:
                    candidate = FocusManager.FindNextElement(FocusNavigationDirection.Left, options);
                    if (candidate == null)
                    {
                        // Wrap the line and try to move to the previous row
                        candidate = FocusManager.FindNextElement(FocusNavigationDirection.Up, options);
                        if (candidate != null)
                        {
                            while (FocusManager.FindNextElement(FocusNavigationDirection.Right, options) != null)
                            {
                                (candidate as Control).Focus(FocusState.Keyboard);
                                candidate = FocusManager.FindNextElement(FocusNavigationDirection.Right, options);
                            }
                        }
                    }
                    break;
                case Windows.System.VirtualKey.Right:
                    candidate = FocusManager.FindNextElement(FocusNavigationDirection.Right, options);
                    if (candidate == null)
                    {
                        // Wrap the line and try to move to the ext row
                        candidate = FocusManager.FindNextElement(FocusNavigationDirection.Down, options);
                        if (candidate != null)
                        {
                            while (FocusManager.FindNextElement(FocusNavigationDirection.Left, options) != null)
                            {
                                (candidate as Control).Focus(FocusState.Keyboard);
                                candidate = FocusManager.FindNextElement(FocusNavigationDirection.Left, options);
                            }
                        }
                    }
                    else
                    {
                        // Handle the last row last item cas
                        // Prevent it to move back to last row right most item
                        var oldItem = FocusManager.GetFocusedElement();
                        (candidate as Control).Focus(FocusState.Keyboard);
                        if (oldItem != FocusManager.FindNextElement(FocusNavigationDirection.Left, options))
                        {
                            candidate = (DependencyObject)oldItem;
                        }
                    }
                    break;
            }

            //Note you should also consider whether is a Hyperlink, WebView, or TextBlock.
            if (candidate != null && candidate is Control)
            {
                (candidate as Control).Focus(FocusState.Keyboard);
                e.Handled = true;
            }
        }
    }
}
