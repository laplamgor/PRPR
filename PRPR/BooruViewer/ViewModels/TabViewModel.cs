using Microsoft.Toolkit.Uwp.UI.Controls;
using PRPR.BooruViewer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.ViewModels
{
    public class TabViewModel
    {
        // Your tabs
        ObservableCollection<TabViewItem> _tabs = new ObservableCollection<TabViewItem>();

        public ObservableCollection<TabViewItem> Tabs
        {
            get
            {
                return _tabs;
            }
        }

        public TabViewModel()
        {
            _tabs.Add(new TabViewItem() { Content = new TabSummary() });
        }
    }
}
