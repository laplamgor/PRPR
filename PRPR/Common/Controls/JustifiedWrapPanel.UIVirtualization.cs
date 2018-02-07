using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PRPR.Common.Controls
{
    public partial class JustifiedWrapPanel
    {
        bool IsItemItsOwnContainer(object item)
        {
            return item is ContentControl && (item as ContentControl).Content == item;
        }

        DependencyObject GetContainerForItem()
        {
            var container = new GridViewItem()
            {
                UseSystemFocusVisuals = true,

                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch
            };

            container.Tapped += Container_Tapped;
            container.KeyUp += Container_KeyUp;

            return container;
        }

        

        void ClearContainerForItem(DependencyObject element, object item)
        {
            if (!IsItemItsOwnContainer(item))
            {
                if (element is ContentControl container)
                {
                    container.Content = null;
                    container.Style = null;
                    container.ContentTemplate = null;
                }
            }
        }

        void PrepareContainerForItem(DependencyObject element, object item)
        {
            if (!IsItemItsOwnContainer(item))
            {
                if (element is ContentControl container)
                {
                    container.Content = item;
                    container.Style = ItemContainerStyle;
                    container.ContentTemplate = ItemTemplate;
                }
            }
        }

        void OnItemsChanged(object e)
        {

        }

        void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {

        }

        void OnItemContainerStyleSelectorChanged(StyleSelector oldItemContainerStyleSelector, StyleSelector newItemContainerStyleSelector)
        {

        }

        void OnItemTemplateChanged(DataTemplate oldItemTemplate, DataTemplate newItemTemplate)
        {

        }


        void OnItemTemplateSelectorChanged(DataTemplateSelector oldItemTemplateSelector, DataTemplateSelector newItemTemplateSelector)
        {

        }

        void OnGroupStyleSelectorChanged(GroupStyleSelector oldGroupStyleSelector, GroupStyleSelector newGroupStyleSelector)
        {

        }

        object ItemFromContainer(DependencyObject container)
        {
            return (container as ContentControl).Content;
        }

        public DependencyObject ContainerFromItem(object item)
        {
            return Containers.FirstOrDefault(o => o.Content == item);
        }


        //int IndexFromContainer(DependencyObject container)
        //{

        //}


        //DependencyObject ContainerFromIndex(int index)
        //{

        //}

        List<ContentControl> RecycledContainers = new List<ContentControl>();

        List<ContentControl> Containers = new List<ContentControl>();


        /// <summary>
        /// Update the virtualization state of all items after changes. Done with optimization.
        /// </summary>
        void RevirtualizeAll()
        {
            if (ItemsSource is IList items)
            {
                var itemsToRealize = new List<object>();
                for (int i = FirstActive; i <= LastActive; i++)
                {
                    if (items.Count > i && i >= 0)
                    {
                        itemsToRealize.Add(items[i]);
                    }
                }

                var containersToRelease = new List<ContentControl>();
                foreach (var container in Containers)
                {
                    if (itemsToRealize.Contains(container.Content))
                    {
                        itemsToRealize.Remove(container.Content);
                    }
                    else
                    {
                        containersToRelease.Add(container);
                    }
                }

                // We do all the realization before the releasing
                // It prevents immeatately reuse of container within same frame
                // It ensures reused containers can have entrance transition
                // instead of Reposition transition across the viewport
                // although might eat a bit more memory
                foreach (var item in itemsToRealize)
                {
                    RealizeItem(item);
                }
                foreach (var container in containersToRelease)
                {
                    RecycleItem(container.Content);
                }
            }

            // Although the Children set (containers) may bot be added/removed in this method,
            // they may still represent another item and therefore need a new size and position
            InvalidateMeasure();
        }

        void RecycleItem(object item)
        {
            var container = ContainerFromItem(item);
            if (container != null)
            {
                ClearContainerForItem(container, item);
                Containers.Remove(container as ContentControl);
                Children.Remove(container as ContentControl);
                RecycledContainers.Add(container as ContentControl);
            }
        }

        void RealizeItem(object item)
        {
            // Check if already realized
            if (ContainerFromItem(item) != null)
            {
                return;
            }


            ContentControl container;
            if (IsItemItsOwnContainer(item))
            {
                container = item as ContentControl;
            }
            else
            {
                // Reuse container if possible
                if (RecycledContainers.Count > 0)
                {
                    container = RecycledContainers.First();
                    RecycledContainers.Remove(container);
                }
                else
                {
                    container = (ContentControl)GetContainerForItem();
                }
            }

            if (container == null)
            {
                // TODO: debug
                return;
            }


            PrepareContainerForItem(container, item);
            Containers.Add(container);
            
            Children.Add(container);
        }

        void RecycleAll()
        {
            this.Children.Clear();
            while (Containers.Count != 0)
            {
                var container = Containers.First();
                if (container != null)
                {
                    ClearContainerForItem(container, container.Content);
                    Containers.Remove(container as ContentControl);
                    RecycledContainers.Add(container as ContentControl);
                }
            }
        }
    }
}
