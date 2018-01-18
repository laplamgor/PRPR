using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.Models
{
    public class ExSearchConfig : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        string x = "https://exhentai.org/?f_doujinshi=1&f_manga=1&f_artistcg=1&f_gamecg=1&f_western=1&f_non-h=1&f_imageset=1&f_cosplay=1&f_asianporn=1&f_misc=1&f_search=loli&f_apply=Apply+Filter";


        private bool _doujinshi = false;
        private bool _manga = false;
        private bool _artistCG = false;
        private bool _gameCG = false;
        private bool _western = false;
        private bool _nonH = true;
        private bool _imageSet = false;
        private bool _cosplay = false;
        private bool _asianPorn = false;
        private bool _misc = false;

        private bool _searchName = true;
        private bool _searchTags = true;
        private bool _searchDescription = true;
        private bool _searchTorrent = false;

        private bool _searchRating = false;
        private int _minRating = 2;

        public bool Doujinshi
        {
            get
            {
                return _doujinshi;
            }

            set
            {
                _doujinshi = value;
                NotifyPropertyChanged(nameof(Doujinshi));
            }
        }

        public bool Manga
        {
            get
            {
                return _manga;
            }

            set
            {
                _manga = value;
                NotifyPropertyChanged(nameof(Manga));
            }
        }

        public bool ArtistCG
        {
            get
            {
                return _artistCG;
            }

            set
            {
                _artistCG = value;
                NotifyPropertyChanged(nameof(ArtistCG));
            }
        }

        public bool GameCG
        {
            get
            {
                return _gameCG;
            }

            set
            {
                _gameCG = value;
                NotifyPropertyChanged(nameof(GameCG));
            }
        }

        public bool Western
        {
            get
            {
                return _western;
            }

            set
            {
                _western = value;
                NotifyPropertyChanged(nameof(Western));
            }
        }

        public bool NonH
        {
            get
            {
                return _nonH;
            }

            set
            {
                _nonH = value;
                NotifyPropertyChanged(nameof(NonH));
            }
        }

        public bool ImageSet
        {
            get
            {
                return _imageSet;
            }

            set
            {
                _imageSet = value;
                NotifyPropertyChanged(nameof(ImageSet));
            }
        }

        public bool Cosplay
        {
            get
            {
                return _cosplay;
            }

            set
            {
                _cosplay = value;
                NotifyPropertyChanged(nameof(Cosplay));
            }
        }

        public bool AsianPorn
        {
            get
            {
                return _asianPorn;
            }

            set
            {
                _asianPorn = value;
                NotifyPropertyChanged(nameof(AsianPorn));
            }
        }

        public bool Misc
        {
            get
            {
                return _misc;
            }

            set
            {
                _misc = value;
                NotifyPropertyChanged(nameof(Misc));
            }
        }

        public int MinRating
        {
            get
            {
                return _minRating;
            }

            set
            {
                _minRating = value;
                NotifyPropertyChanged(nameof(MinRating));
            }
        }

        public bool SearchRating
        {
            get
            {
                return _searchRating;
            }

            set
            {
                _searchRating = value;
                NotifyPropertyChanged(nameof(SearchRating));
            }
        }

        public bool SearchTorrent
        {
            get
            {
                return _searchTorrent;
            }

            set
            {
                _searchTorrent = value;
                NotifyPropertyChanged(nameof(SearchTorrent));
            }
        }

        public bool SearchDescription
        {
            get
            {
                return _searchDescription;
            }

            set
            {
                _searchDescription = value;
                NotifyPropertyChanged(nameof(SearchDescription));
            }
        }

        public bool SearchTags
        {
            get
            {
                return _searchTags;
            }

            set
            {
                _searchTags = value;
                NotifyPropertyChanged(nameof(SearchTags));
            }
        }

        public bool SearchName
        {
            get
            {
                return _searchName;
            }

            set
            {
                _searchName = value;
                NotifyPropertyChanged(nameof(SearchName));
            }
        }

        public override string ToString()
        {
            return String.Concat(
                _doujinshi ? "f_doujinshi=on&" : "",
                _manga ? "f_manga=on&" : "",
                _artistCG ? "f_artistcg=on&" : "",
                _gameCG ? "f_gamecg=on&" : "",
                _western ? "f_western=on&" : "",
                _nonH ? "f_non-h=on&" : "",
                _imageSet ? "f_imageset=on&" : "",
                _cosplay ? "f_cosplay=on&" : "",
                _asianPorn ? "f_asianporn=on&" : "",
                _misc ? "f_misc=on&" : ""
                ).TrimEnd('&') + "&f_apply=Apply+Filter";
        }
        
    }
}
