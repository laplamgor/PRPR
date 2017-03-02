using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;

namespace PRPR.BooruViewer.Models
{
    [XmlType("comments")]
    public class Comments : ObservableCollection<Comment>
    {
        public static async Task<Comments> GetComments(int postId)
        {
            HttpClient httpClient = new HttpClient();
            var xml = await httpClient.GetStringAsync(new Uri($"https://yande.re/comment.xml?post_id={postId}"));
            var p = Comments.ReadFromXml(xml);
            return p;
        }

        private static Comments ReadFromXml(string xml)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Comments));
            using (StringReader reader = new StringReader(xml))
            {
                var result = (Comments)deserializer.Deserialize(reader);

                // Reverse 
                for (int i = 0; i < result.Count; i++)
                    result.Move(result.Count - 1, i);


                return result;
            }
        }
    }
}
