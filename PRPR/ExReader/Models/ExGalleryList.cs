using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using PRPR.ExReader.Services;
using PRPR.ExReader.Models.Global;
using System.ComponentModel;

namespace PRPR.ExReader.Models
{
    public class ExGalleryList : ObservableCollection<ExGallery>, ISupportIncrementalLoading, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
        public static async Task<ExGalleryList> DownloadGalleryListAsync(int pagenumber, string uri)
        {
            try
            {
                // Get page html
                var htmlStr = await ExClient.GetStringWithExCookie($"{uri}&page={pagenumber}", "dm_t-" + $"xl_{ExSettings.Current.xl}");
                
                var e = ExGalleryList.GetExGalleryListFromHtml(htmlStr);
                e.uri = uri;
                return e;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        


        public int EstimatedTotalCount {
            get
            {
                if (CurrentPageNumber >= PageCount - 1)
                {
                    // The number is correctly calculated if it is the last page
                    return Count;
                }
                else
                {
                    // Estimate the count by page number only
                    return 25 * PageCount;
                }
            }
        }



        public int PageCount { get; set; } = 0;

        public int CurrentPageNumber { get; set; } = 0;
        
        public string uri { get; set; } = "";







        #region ISupportIncrementalLoading

        public bool HasMoreItems
        {
            get
            {
                return this.CurrentPageNumber < PageCount - 1;
            }
        }


        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }


        #endregion






        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var nextPageList = await DownloadGalleryListAsync(this.CurrentPageNumber + 1, this.uri);
                foreach (var item in nextPageList)
                {
                    this.Add(item);
                }
                
                this.CurrentPageNumber = nextPageList.CurrentPageNumber;
                this.PageCount = nextPageList.PageCount;
                NotifyPropertyChanged(nameof(EstimatedTotalCount));
                NotifyPropertyChanged(nameof(Count));

                return new LoadMoreItemsResult { Count = (uint)nextPageList.Count };
            }
            catch (Exception ex)
            {
                return new LoadMoreItemsResult { Count = 0 };
            }
        }
        
        private static ExGalleryList GetExGalleryListFromHtml(string htmlSource)
        {
            if (htmlSource == null)
            {
                return null;
            }

            var l = new ExGalleryList();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(htmlSource);


            HtmlNodeCollection galleryNodes;
            try
            {
                galleryNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='id1']");
                foreach (var node in galleryNodes)
                {
                    l.Add(ExGallery.GetGalleryListItemFromNode(node));
                }

                l.PageCount = ReadPageCount(htmlDocument);
                l.CurrentPageNumber = ReadCurrentPageNumber(htmlDocument);
            }
            catch (Exception ex)
            {

            }

            return l;
        }
        
        private static int ReadPageCount(HtmlDocument htmlDocument)
        {
            var c = htmlDocument.DocumentNode.SelectSingleNode("//table[@class='ptt']//td[last()-1]/a");
            if (c != null)
            {
                return int.Parse(c.InnerText);
            }
            else
            {
                return 0;
            }
        }


        private static int ReadCurrentPageNumber(HtmlDocument htmlDocument)
        {
            var c = htmlDocument.DocumentNode.SelectSingleNode("//table[@class='ptt']//td[@class='ptds']/a");
            if (c != null)
            {
                return int.Parse(c.InnerText) - 1;
            }
            else
            {
                return 0;
            }

        }
    }
}
