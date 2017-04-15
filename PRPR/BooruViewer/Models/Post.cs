using PRPR.BooruViewer.Services;
using PRPR.Common.Models;
using PRPR.Common.Views.Controls;
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
    public class Post : IImageWallItemImage
    {
        public static string test = @"<?xml version=""1.0"" encoding=""utf-16""?>
<post id=""361174"" 
tags=""dress gothic_lolita inaba_sunimi kanzaki_ranko lolita_fashion the_idolm@ster the_idolm@ster_cinderella_girls"" created_at=""1468339249"" 
creator_id=""25882"" author=""Mr_GT"" change=""1916218"" 
source=""http://i3.pixiv.net/img-original/img/2016/07/13/00/58/28/57878838_p0.png"" score=""0"" 
md5=""2795ff2560dd993ab027443d40f5dda7"" file_size=""2432738"" 
file_url=""https://files.yande.re/image/2795ff2560dd993ab027443d40f5dda7/yande.re%20361174%20dress%20gothic_lolita%20inaba_sunimi%20kanzaki_ranko%20lolita_fashion%20the_idolm%40ster%20the_idolm%40ster_cinderella_girls.png"" 
is_shown_in_index=""true"" preview_url=""https://assets.yande.re/data/preview/27/95/2795ff2560dd993ab027443d40f5dda7.jpg"" 
preview_width=""106"" preview_height=""150"" actual_preview_width=""213"" actual_preview_height=""300"" 
sample_url=""https://files.yande.re/sample/2795ff2560dd993ab027443d40f5dda7/yande.re%20361174%20sample%20dress%20gothic_lolita%20inaba_sunimi%20kanzaki_ranko%20lolita_fashion%20the_idolm%40ster%20the_idolm%40ster_cinderella_girls.jpg"" 
sample_width=""1063"" sample_height=""1500"" sample_file_size=""497886"" 
jpeg_url=""https://files.yande.re/jpeg/2795ff2560dd993ab027443d40f5dda7/yande.re%20361174%20dress%20gothic_lolita%20inaba_sunimi%20kanzaki_ranko%20lolita_fashion%20the_idolm%40ster%20the_idolm%40ster_cinderella_girls.jpg"" 
jpeg_width=""1200"" jpeg_height=""1694"" jpeg_file_size=""688109"" 
rating=""s"" has_children=""false"" parent_id="""" status=""active"" 
width=""1200"" height=""1694"" is_held=""false"" frames_pending_string="""" frames_string=""""/>
";

        internal object Where(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }

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
        public string parent_id { get; set; }


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
    }


    public enum PostImageVersion
    {
        Preview,
        Sample,
        Jpeg,
        Source
    }
}
