using HtmlAgilityPack;
using PRPR.ExReader.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.Models
{
    public class ExImage : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        public string Link { get; set; }

        public string Thumb { get; set; }

        private string _imageSource = "";
        public string ImageSource
        {
            get
            {
                if (_imageSource == "")
                {
                    LoadFromInternetAsync();
                }
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                NotifyPropertyChanged(nameof(ImageSource));
            }
        }
        

        public async Task LoadFromInternetAsync()
        {
            if (String.IsNullOrEmpty(_imageSource))
            {
                try
                {
                    // Get page html
                    var htmlSource = await ExClient.GetStringWithExCookie($"{this.Link}");
                    this.ImageSource = GetImageUriFromHtml(htmlSource);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static string GetImageUriFromHtml(string htmlSource)
        {

            if (htmlSource == null)
            {
                return null;
            }
            
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(htmlSource);



            var imgNode = htmlDocument.DocumentNode.SelectSingleNode("//img[@id='img']");

            if (imgNode != null)
            {
                return imgNode.GetAttributeValue("src", "");
            }
            else
            {
                throw new Exception("Cannot prase the image uri");
            }
        }

        
    }
}
