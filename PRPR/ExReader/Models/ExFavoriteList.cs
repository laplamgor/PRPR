using HtmlAgilityPack;
using PRPR.ExReader.Services;
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

namespace PRPR.ExReader.Models
{
    public class ExFavoriteList : ObservableCollection<ExFavorite>, ISupportIncrementalLoading
    {
        public int PageCount { get; set; } = 0;

        public int CurrentPageNumber { get; set; } = 0;



        public bool HasMoreItems
        {
            get
            {
                return this.CurrentPageNumber < PageCount;
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
                var nextPageList = await DownloadFavoritesAsync(this.CurrentPageNumber + 1);
                foreach (var item in nextPageList)
                {
                    this.Add(item);
                }

                this.CurrentPageNumber = nextPageList.CurrentPageNumber;
                this.PageCount = nextPageList.PageCount;

                return new LoadMoreItemsResult { Count = (uint)nextPageList.Count };
            }
            finally
            {
                //;
            }
        }

        public static async Task<ExFavoriteList> DownloadFavoritesAsync(int pagenumber)
        {
            try
            {
                // Get page html
                if (pagenumber == 1)
                {
                    var htmlStr = await ExClient.GetStringWithExCookie($"https://exhentai.org/favorites.php", "dm_l");
                    return ExFavoriteList.FromHtml(htmlStr);
                }
                else
                {
                    var htmlStr = await ExClient.GetStringWithExCookie($"https://exhentai.org/favorites.php?page= {pagenumber - 1}");
                    return ExFavoriteList.FromHtml(htmlStr);
                }


                //return await client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot download favorite list");
            }
        }

        private static ExFavoriteList FromHtml(string htmlSource)
        {
            if (htmlSource == null)
            {
                return null;
            }

            var l = new ExFavoriteList();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(htmlSource);

            try
            {
                HtmlNodeCollection favoriteItemNodes = htmlDocument.DocumentNode.SelectNodes("//tr[@class]");
                foreach (var node in favoriteItemNodes)
                {
                    l.Add(ExFavorite.FromHtmlNode(node));
                }

                l.PageCount = ReadPageCount(htmlDocument);
                l.CurrentPageNumber = ReadCurrentPageNumber(htmlDocument);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot load favorite list html");
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
                return int.Parse(c.InnerText);
            }
            else
            {
                return 0;
            }

        }
    }
}
