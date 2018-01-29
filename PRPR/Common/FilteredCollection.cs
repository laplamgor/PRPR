using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace PRPR.Common
{
    public class FilteredCollection<T, TSource> : ObservableCollection<T>, ISupportIncrementalLoading where TSource : IEnumerable<T>, ISupportIncrementalLoading
    {
        
        public FilteredCollection(TSource source, IConfigableFilter<T> filter)
        {
            this._source = source;
            this.filter = filter;
            this.filter.PropertyChanged += Filter_PropertyChanged;
            Refilter();


        }

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Function")
            {
                Refilter();

            }
        }

        private IConfigableFilter<T> filter = null;
        
        private TSource _source = default(TSource);

        public TSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                Refilter();
            }
        }


        private void Refilter()
        {
            this.Clear();
            if (Source != null)
            {
                var newSourceItems = Source.Where(filter.Function);

                if (this.Count == 0)
                {
                    foreach (var item in newSourceItems)
                    {
                        this.Add(item);
                    }
                }
                else
                {
                    // There are other items loaded during this download
                    // Prevent duplicate items
                }
            }
            else
            {

            }
        }

        public bool HasMoreItems
        {
            get
            {
                return (this.Source != null && this.Source.HasMoreItems) || (this.Source != null && this.Source.Where(filter.Function).Count()> this.Count());
            }
        }



        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }




        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var oldFilteredCount = this.Count;
                var oldCount = this.Count();

                if (oldCount >= Source.Where(filter.Function).Count())
                {
                    var x = await Source.LoadMoreItemsAsync(count);
                }
                var newCount = this.Source.Count();
                var newSourceItems = Source.Where(filter.Function).Skip(oldCount);

                if (oldCount == this.Count)
                {
                    foreach (var item in newSourceItems)
                    {
                        this.Add(item);
                    }

                    return new LoadMoreItemsResult { Count = (uint)(this.Count - oldFilteredCount) };
                }
                else
                {
                    // There are other items loaded during this download
                    // Prevent duplicate items
                    return new LoadMoreItemsResult { Count = 0 };
                }
            }
            catch (Exception ex)
            {
                return new LoadMoreItemsResult { Count = 0 };
            }
        }

    }
}
