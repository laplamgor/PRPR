using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.Models
{
    public class ExComment
    {
        public DateTime PostDate { get; set; }

        public string Author { get; set; }

        public int Score { get; set; } = 0;

        public string Content { get; set; }

        public bool IsUploaderComment { get; set; }



        public static ExComment FromNode(HtmlNode node)
        {
            var c = new ExComment()
            {
                Author = ReadAuthor(node),
                PostDate = ReadPostDate(node),
                Content = ReadContent(node),
                IsUploaderComment = ReadIsUploaderComment(node)
            };


            if (!c.IsUploaderComment)
            {
                c.Score = ReadScore(node);
            }
            return c;
        }

        private static string ReadContent(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//div[@class='c6']");

            if (c != null)
            {
                //var s = WebUtility.HtmlDecode(c.InnerText);
                var s = ReadInnerText(c);
                return s;

            }
            else
            {
                throw new Exception("Cannot parse comment content");
            }
        }

        static string ReadInnerText(HtmlNode node)
        {
            string s = "";

            foreach (var childNode in node.ChildNodes)
            {
                if (childNode is HtmlTextNode textNode)
                {
                    s += textNode.Text;
                }
                else if (childNode.Name == "br")
                {
                    s += '\n';
                }
                else
                {
                    s += ReadInnerText(childNode);
                }
            }
            return s;
        }

        private static int ReadScore(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//div[@class='c5 nosel']/span");

            if (c != null)
            {
                return int.Parse(c.InnerText);
            }
            else
            {
                throw new Exception("Cannot parse comment score");
            }
        }
        
        private static DateTime ReadPostDate(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//div[@class='c3']/text()");

            if (c != null)
            {
                var s = WebUtility.HtmlDecode(c.InnerText).Replace("Posted on", "").Replace("UTC by:", "");
                return DateTimeOffset.Parse(s).DateTime;
            }
            else
            {
                throw new Exception("Cannot parse comment score");
            }
        }

        private static string ReadAuthor(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//div[@class='c3']/a");

            if (c != null)
            {
                return (c.InnerText);
            }
            else
            {
                throw new Exception("Cannot parse comment author");
            }
        }

        private static bool ReadIsUploaderComment(HtmlNode node)
        {
            var c = node.SelectSingleNode(".//a[@name='ulcomment']");

            return (c != null);
        }
    }
}
