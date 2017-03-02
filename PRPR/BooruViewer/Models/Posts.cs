using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;

namespace PRPR.BooruViewer.Models
{
    public class Posts : ObservableCollection<Post>, ISupportIncrementalLoading
    {
        const int limit = 100;

        public int TotalCount { get; set; }

        
        public int Offset { get; set; } = 0;

        
        public string Uri { get; set; }

        public virtual bool HasMoreItems
        {
            get
            {
                return this.Offset + this.Count < this.TotalCount;
            }
        }

        public virtual IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }




        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var nextPagePosts = await DownloadPostsAsync((this.Offset + this.Count) / limit + 1, this.Uri);
                foreach (var item in nextPagePosts)
                {
                    this.Add(item);
                }

                this.TotalCount = nextPagePosts.TotalCount;
                //this.Offset = 0;

                return new LoadMoreItemsResult { Count = (uint)nextPagePosts.Count };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        

        public static async Task<Posts> DownloadPostsAsync(int page, string uri)
        {
            HttpClient httpClient = new HttpClient();
            var xml = await httpClient.GetStringAsync(new Uri($"{uri}&page={page}&limit={limit}"));
            var p = Posts.ReadFromXml(xml);
            p.Uri = uri;
            return p;
        }

        private static Posts ReadFromXml(string xml)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SerializablePosts));
            using (StringReader reader = new StringReader(xml))
            {
                var result = (SerializablePosts)deserializer.Deserialize(reader);
                var p = new Posts() { TotalCount = result.TotalCount, Offset = result.Offset };
                foreach (var item in result.Items)
                {
                    p.Add(item);
                }
                return p;
            }
        }
    }



    [XmlType("posts")]

    public class SerializablePosts
    {

        [XmlElement("post")]
        public List<Post> Items
        {
            get; set;
        }


        [XmlAttribute("count")]
        public int TotalCount { get; set; }


        [XmlAttribute("offset")]
        public int Offset { get; set; } = 0;
    }
}