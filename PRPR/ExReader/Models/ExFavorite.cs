using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace PRPR.ExReader.Models
{
    public class ExFavorite
    {

        public string Gid { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Link { get; set; }
        
        public string Thumb { get; set; }

        public float Rating { get; set; }

        public string Published { get; set; }

        public string Favorited { get; set; }



        internal static ExFavorite FromHtmlNode(HtmlNode node)
        {
            var f = new ExFavorite();

            f.Thumb = ReadThumb(node);
            f.Title = ReadTitle(node);
            f.Link = ReadLink(node);
            f.Published = ReadPublished(node);

            return f;
        }

        private static string ReadPublished(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//td[@class='itd'][last()]/text()");
            if (c != null)
            {
                return WebUtility.HtmlDecode(c.InnerHtml).Trim();
            }
            else
            {
                return null;
            }
        }

        private static string ReadTitle(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//div[@class='it5']/a/text()");
            if (c != null)
            {
                return WebUtility.HtmlDecode(c.InnerHtml).Trim();
            }
            else
            {
                return null;
            }
        }

        private static string ReadLink(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//div[@class='it5']/a");
            if (c != null)
            {
                return c.GetAttributeValue("href", "");
            }
            else
            {
                return null;
            }
        }

        private static string ReadThumb(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//div//img");
            if (c != null)
            {
                return c.GetAttributeValue("src", "");
            }
            else
            {
                return null;
            }
        }
    }
}
