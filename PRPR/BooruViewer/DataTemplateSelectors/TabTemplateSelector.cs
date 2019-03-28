using PRPR.BooruViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PRPR.BooruViewer.DataTemplateSelectors
{
    /// <summary>
    /// This class is not used currently 
    /// because the TabView does not support DataTemplateSelector for TabItem content
    /// As a result, the tab template have to contains all visual elements for all tab types
    /// </summary>
    public class TabTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PostSummaryTemplate { get; set; }
        public DataTemplate PostListTemplate { get; set; }
        public DataTemplate PostDetailTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var uiElement = container as UIElement;
            if (uiElement == null)
            {
                return base.SelectTemplateCore(item, container);
            }

            if (item is TabSummary)
            {
                return PostSummaryTemplate;
            }
            else if(item is TabPostList)
            {
                return PostListTemplate;
            }
            else
            {
                return PostDetailTemplate;
            }
        }
    }
}
