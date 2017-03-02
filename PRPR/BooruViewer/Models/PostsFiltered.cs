using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace PRPR.BooruViewer.Models
{
    public class PostsFiltered : Posts
    {

        public PostsFiltered(Posts postsSource, Func<Post, bool> filter)
        {
            this.PostsSource = postsSource;
            this.Filter = filter;

            this.Clear();
            var newSourceItems = postsSource.Where(Filter);
            foreach (var item in newSourceItems)
            {
                this.Add(item);
            }
        }
        public override bool HasMoreItems
        {
            get
            {
                return this.PostsSource != null && this.PostsSource.HasMoreItems;
            }
        }

        public Posts PostsSource { get; set; } = null;

        public Func<Post, bool> Filter { get; set; } = null;

        public override IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }




        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var oldFilteredCount = this.Count;

                var oldCount = this.PostsSource.Count;
                var x = await PostsSource.LoadMoreItemsAsync(count);
                var newCount = this.PostsSource.Count;
                var newSourceItems = PostsSource.Skip(oldCount).Where(Filter);


                foreach (var item in newSourceItems)
                {
                    this.Add(item);
                }

                return new LoadMoreItemsResult { Count = (uint)(this.Count - oldFilteredCount) };
            }
            catch (Exception ex)
            {
                return new LoadMoreItemsResult { Count = 0 };
            }
        }

    }
}
