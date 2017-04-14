using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PRPR.Common.Controls
{
    public class CustomItemsControl
    {









        bool IsItemItsOwnContainer(object item)
        {
            return item is ContentControl && (item as ContentControl).Content == item;
        }

        DependencyObject GetContainerForItem()
        {
            return new ContentControl();
        }

        void ClearContainerForItem(DependencyObject element, object item)
        {
            if (!IsItemItsOwnContainer(item))
            {
                if (element is ContentControl container)
                {
                    container.Content = null;
                    container.ContentTemplate = null;
                    container.Style = null;
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
                    container.ContentTemplate = ItemTemplate;
                    container.Style = ItemContainerStyle;
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

        DependencyObject ContainerFromItem(object item)
        {
            return Containers.FirstOrDefault(o => o.Content == item);
        }


        //int IndexFromContainer(DependencyObject container)
        //{

        //}


        //DependencyObject ContainerFromIndex(int index)
        //{

        //}
        



        object ItemsSource { get; set; }

        DataTemplate ItemTemplate { get; set; }

        Style ItemContainerStyle { get; set; }






        List<ContentControl> RecycledContainers = new List<ContentControl>();

        List<ContentControl> Containers = new List<ContentControl>();


        void RecycleItem(object item)
        {
            var container = ContainerFromItem(item);
            ClearContainerForItem(container, item);
            Containers.Remove(container as ContentControl);
            RecycledContainers.Add(container as ContentControl);
        }

        void RealizeItem(object item)
        {
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


            PrepareContainerForItem(container, item);
            Containers.Add(container);
        }
    }
}
