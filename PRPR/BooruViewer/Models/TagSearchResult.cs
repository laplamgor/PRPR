using PRPR.BooruViewer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PRPR.BooruViewer.Models
{
    [XmlType("tags")]
    public class TagSearchResult : ObservableCollection<Tag>
    {
        public string Key { get; set; }

        

        public static async Task<TagSearchResult> DownloadTagsAsync(string key)
        {
            HttpClient httpClient = new HttpClient();
            var xml = await httpClient.GetStringAsync(new Uri($"{YandeClient.HOST}/tag.xml?order=count&limit=10&name={key}"));
            var p = ReadFromXml(xml);
            p.Key = key;
            return p;
        }

        private static TagSearchResult ReadFromXml(string xml)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(TagSearchResult));
            using (StringReader reader = new StringReader(xml))
            {
                var result = (TagSearchResult)deserializer.Deserialize(reader);
                return result;
            }
        }
    }
}
