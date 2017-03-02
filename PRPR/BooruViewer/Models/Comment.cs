using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PRPR.BooruViewer.Models
{
    [XmlType("comment")]
    public class Comment
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        
        [XmlAttribute("created_at")]
        public string created_at { get; set; }


        [XmlAttribute("post_id")]
        public int PostId { get; set; }


        [XmlAttribute("creator_id")]
        public int CreatorId { get; set; }


        [XmlAttribute("creator")]
        public string Creator { get; set; }


        [XmlAttribute("body")]
        public string Body { get; set; }
    }
}
