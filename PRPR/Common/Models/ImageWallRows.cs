using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace PRPR.Common.Models
{
    public class ImageWallRows<T> : ObservableCollection<ImageWallRow<T>>, ISupportIncrementalLoading, INotifyPropertyChanged, IImageWallRows where T : IImageWallItemImage
    {
        #region INotifyPropertyChanged
        public new event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        

        // Posts part
        private ObservableCollection<T> _itemsSource = null;

        public ObservableCollection<T> ItemsSource
        {
            get
            {
                return _itemsSource;
            }
            set
            {
                if (_itemsSource != null)
                {
                    _itemsSource.CollectionChanged -= _postsSource_CollectionChanged;
                }

                _itemsSource = value;

                Resize();
                NotifyPropertyChanged(nameof(HasMoreItems));

                _itemsSource.CollectionChanged += _postsSource_CollectionChanged;
            }
        }
        
        public object GetSource()
        {
            return ItemsSource;
        }

        private void _postsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                Resize();
                NotifyPropertyChanged(nameof(HasMoreItems));
            }
        }

        public bool HasMoreItems
        {
            get
            {
                return ItemsSource != null && 
                    ((ItemsSource as ISupportIncrementalLoading).HasMoreItems || this.Sum(o => o.Count) < ItemsSource.Count);
            }
        }

        


        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }
        
        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            if (RowHeight == 0)
            {
                return new LoadMoreItemsResult { Count = 0 };
            }

            try
            {
                // Load some new posts
                var rowOldCount = this.Count;
                var oldCount = this.Sum(o => o.Count);
                await (ItemsSource as ISupportIncrementalLoading).LoadMoreItemsAsync(count);
                var itemsNotInAnyRow = ItemsSource.Skip(oldCount);

                // Put them in the new rows
                AddRows(itemsNotInAnyRow);


                NotifyPropertyChanged(nameof(HasMoreItems));
                return new LoadMoreItemsResult { Count = (uint)(this.Count - rowOldCount) };
            }
            catch(Exception ex)
            {
                return new LoadMoreItemsResult { Count = 0 };
            }
        }

        public double RowHeight { get; set; } = 0;

        public double RowWidth
        {
            get
            {
                return _rowWidth;
            }

            set
            {
                _rowWidth = value;
            }
        }

        private double _rowWidth = 0;

        public void Resize()
        {
            this.Clear();
            AddRows(ItemsSource);
        }


        private void AddRows(IEnumerable<T> newPosts)
        {
            var maxTotalWidth = RowWidth;
            var height = RowHeight;
            int count = 0;

            foreach (var post in newPosts)
            {
                if (this.Count == 0)
                {
                    this.Add(new ImageWallRow<T>() { MaxRowWidth = maxTotalWidth, RowHeight = height });
                    count++;
                }

                // Try to place an item
                if (maxTotalWidth - this.Last().RowWidth < Math.Min(maxTotalWidth, post.PreferredWidth * height / post.PreferredHeight) / 2.0)
                {
                    var scaleFactor = this.Last().MaxRowWidth / this.Last().RowWidth;

                    foreach (var rowItem in this.Last())
                    {
                        rowItem.DisplayWidth *= scaleFactor;
                    }
                    this.Add(new ImageWallRow<T>() { MaxRowWidth = maxTotalWidth, RowHeight = height });
                    count++;
                }

                var n = new ImageWallItem<T>() { ItemSource = post };
                n.DisplayHeight = height;
                n.DisplayWidth = Math.Min(maxTotalWidth, post.PreferredWidth * height / post.PreferredHeight);

                this.Last().Add(n);

                // Check whether the row is full after adding the item
                if (this.Last().RowWidth > maxTotalWidth)
                {
                    var scaleFactor = this.Last().MaxRowWidth / this.Last().RowWidth;

                    foreach (var rowItem in this.Last())
                    {
                        rowItem.DisplayWidth *= scaleFactor;
                    }
                }
            }
        }
    }


    public interface IImageWallRows
    {
        object GetSource();

        void Resize();

        double RowWidth { get; set; }
        double RowHeight { get; set; }
    }
}