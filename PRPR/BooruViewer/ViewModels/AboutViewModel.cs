using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Models.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.ViewModels
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        Posts _testPosts = null;

        public Posts TestPosts
        {
            get => _testPosts;
            set
            {
                _testPosts = value;
                NotifyPropertyChanged(nameof(TestPosts));
            }
        }

        public async Task LoadTestDataAsync()
        {
            TestPosts = await Posts.DownloadPostsAsync(1, $"https:{""}//yande.re/post.xml?tags=vote:3:{YandeSettings.Current.UserName}+order:vote");
        }
    }
}
