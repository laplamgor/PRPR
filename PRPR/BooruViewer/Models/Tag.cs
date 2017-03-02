using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PRPR.BooruViewer.Models
{
    [XmlType("tag")]
    public class Tag
    {

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public int Type { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlAttribute("ambiguous")]
        public bool Ambiguous { get; set; }
        
        [XmlIgnore]
        public List<string> Aliases { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
