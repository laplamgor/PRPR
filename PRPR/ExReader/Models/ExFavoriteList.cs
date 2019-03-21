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
    public class ExFavoriteList : ExGalleryList
    {
        public static async Task<ExFavoriteList> DownloadFavoritesAsync(int pagenumber, ExFavoriteSortingMode sortingMode)
        {
            try
            {
                // Get page html
                if (pagenumber == 1)
                {
                    var htmlStr = await ExClient.GetStringWithExCookie($"https://exhentai.org/favorites.php?inline_set={SORTING_STRING[(int)sortingMode]}-https://exhentai.org/favorites.php?inline_set-dm_e", $"");
                    return ExFavoriteList.FromHtml(htmlStr, sortingMode);
                }
                else
                {
                    var htmlStr = await ExClient.GetStringWithExCookie($"https://exhentai.org/favorites.php?page={pagenumber - 1}&inline_set={SORTING_STRING[(int)sortingMode]}-dm_e", $"");
                    return ExFavoriteList.FromHtml(htmlStr, sortingMode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot download favorite list");
            }
        }

        private static ExFavoriteList FromHtml(string htmlSource, ExFavoriteSortingMode sortingMode)
        {
            if (htmlSource == null)
            {
                return null;
            }

            var l = new ExFavoriteList();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(htmlSource);


            HtmlNodeCollection galleryNodes;
            try
            {
                var tableNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@class='itg glt']");
                galleryNodes = tableNode.SelectNodes("//tr[./td/@class='gl1e']");
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
                return int.Parse(c.InnerText);
            }
            else
            {
                return 0;
            }
        }

        public ExFavoriteSortingMode SortingMode = ExFavoriteSortingMode.Favorited;



        private static readonly IReadOnlyList<string> SORTING_STRING = new List<string>() { "fs_f", "fs_p" }.AsReadOnly();
    }

    public enum ExFavoriteSortingMode
    {
        Favorited = 0,
        Published = 1
    }

}
