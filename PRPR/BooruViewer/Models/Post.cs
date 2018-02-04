using PRPR.BooruViewer.Services;
using PRPR.Common.Controls;
using PRPR.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PRPR.BooruViewer.Models
{
    [XmlType("post")]
    public class Post : IJustifiedWrapPanelItem
    {
        /*
<post id="431721" tags="ass breasts cameltoe christmas nipples no_bra pantsu string_panties tenzeru thighhighs topless" created_at="1517064967" updated_at="1517064969" 
creator_id="25882" approver_id="" author="Mr_GT" change="2277917" source="https://i.pximg.net/img-original/img/2017/12/29/07/19/48/66514457_p1.jpg" score="46" 
md5="163cb181d72511897fb445aee5b54c87" file_size="8034985" file_ext="jpg" file_url="https://files.yande.re/image/163cb181d72511897fb445aee5b54c87/yande.re%20431721%20ass%20breasts%20cameltoe%20christmas%20nipples%20no_bra%20pantsu%20string_panties%20tenzeru%20thighhighs%20topless.jpg" 
is_shown_in_index="true" preview_url="https://assets.yande.re/data/preview/16/3c/163cb181d72511897fb445aee5b54c87.jpg" 
preview_width="96" preview_height="150" actual_preview_width="192" actual_preview_height="300" 
sample_url="https://files.yande.re/sample/163cb181d72511897fb445aee5b54c87/yande.re%20431721%20sample%20ass%20breasts%20cameltoe%20christmas%20nipples%20no_bra%20pantsu%20string_panties%20tenzeru%20thighhighs%20topless.jpg" 
sample_width="961" sample_height="1500" sample_file_size="373313" 
jpeg_url="https://files.yande.re/image/163cb181d72511897fb445aee5b54c87/yande.re%20431721%20ass%20breasts%20cameltoe%20christmas%20nipples%20no_bra%20pantsu%20string_panties%20tenzeru%20thighhighs%20topless.jpg" 
jpeg_width="2976" jpeg_height="4644" jpeg_file_size="0" rating="q" is_rating_locked="false" 
has_children="false" parent_id="" status="active" is_pending="false" width="2976" height="4644" is_held="false" 
frames_pending_string="" frames_string="" is_note_locked="false" last_noted_at="0" last_commented_at="0"/>
        */


        public static Post FromXml(string xml)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Post));
            using (StringReader reader = new StringReader(xml))
            {
                return (Post)deserializer.Deserialize(reader);
            }
        }

        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, this);
                return writer.ToString();
            }
        }



        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("tags")]
        public string Tags { get; set; }

        public ObservableCollection<TagDetail> TagItems
        {
            get
            {
                var x = new ObservableCollection<TagDetail>();
                if (!String.IsNullOrEmpty(Tags))
                {
                    var tagNames = Tags.Split(' ');
                    foreach (var tagName in tagNames)
                    {
                        if (tagName != "")
                        {
                            TagDetail tag = null;
                            if (TagDataBase.AllTags.TryGetValue(tagName, out tag))
                            {
                                x.Add(tag);
                            }
                        }
                    }
                }
                x = new ObservableCollection<TagDetail>(x.OrderBy(o => (uint)o.Type));
                return x;
            }
        }


        [XmlAttribute("created_at")]
        public long created_at { get; set; }


        [XmlIgnore]
        public DateTime CreatedAtUtc
        {
            get
            {
                var time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                
                return time.AddSeconds(created_at); ;
            }
        }

        [XmlAttribute("creator_id")]
        public string CreatorId { get; set; }

        [XmlAttribute("author")]
        public string Author { get; set; }

        [XmlAttribute("change")]
        public int change { get; set; }

        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("file_ext")]
        public string FileExtension { get; set; }


        [XmlAttribute("score")]
        public string Score { get; set; }

        [XmlAttribute("md5")]
        public string Md5 { get; set; }

        [XmlAttribute("file_size")]
        public long FileSize { get; set; }

        [XmlAttribute("file_url")]
        public string FileUrl { get; set; }

        [XmlAttribute("is_shown_in_index")]
        public bool IsShownInIndex { get; set; }

        [XmlAttribute("preview_url")]
        public string PreviewUrl { get; set; }

        [XmlAttribute("preview_width")]
        public int PreviewWidth { get; set; }

        [XmlAttribute("preview_height")]
        public int PreviewHeight { get; set; }

        [XmlAttribute("actual_preview_width")]
        public int ActualPreviewWidth { get; set; }

        [XmlAttribute("actual_preview_height")]
        public int ActualPreviewHeight { get; set; }
        
        [XmlAttribute("sample_url")]
        public string SampleUrl { get; set; }

        [XmlAttribute("sample_width")]
        public int SampleWidth { get; set; }

        [XmlAttribute("sample_height")]
        public int SampleHeight { get; set; }

        [XmlAttribute("sample_file_size")]
        public int SampleFileSize { get; set; }


        [XmlAttribute("jpeg_url")]
        public string JpegUrl { get; set; }

        [XmlAttribute("jpeg_width")]
        public int JpegWidth { get; set; }

        [XmlAttribute("jpeg_height")]
        public int JpegHeight { get; set; }

        [XmlAttribute("jpeg_file_size")]
        public int JpegFileSize { get; set; }

        [XmlAttribute("rating")]
        public string Rating { get; set; }

        [XmlAttribute("has_children")]
        public bool HasChildren { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("parent_id")]
        public string ParentId { get; set; }


        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("is_held")]
        public bool IsHeld { get; set; }

        [XmlAttribute("frames_pending_string")]
        public string FramesPendingString { get; set; }

        [XmlAttribute("frames_string")]
        public string FramesString { get; set; }




        [XmlIgnore]
        public bool IsJpeg
        {
            get
            {
                return this.JpegFileSize == 0;
            }
        }

        public double PreferredWidth
        {
            get
            {
                return Width;
            }
        }

        public double PreferredHeight
        {
            get
            {
                return Height;
            }
        }

        public double PreferredRatio
        {
            get
            {
                return (double)Width / Height;
            }
        }
    }


    public enum PostImageVersion
    {
        Preview,
        Sample,
        Jpeg,
        Source
    }
}
