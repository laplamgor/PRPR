using PRPR.BooruViewer.Services;
using PRPR.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.Models
{
    public class PostFilter : INotifyPropertyChanged, IConfigableFilter<Post>
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion




        private bool _isFilterSafe = true;

        private bool _isFilterQuestionable = false;

        private bool _isFilterExplicit = false;


        private bool _isFilterHorizontal = true;

        private bool _isFilterVertical = true;


        private bool _isFilterAllowHidden = false;




        public bool IsFilterSafe
        {
            get
            {
                return _isFilterSafe;
            }

            set
            {
                _isFilterSafe = value;
                NotifyPropertyChanged(nameof(IsFilterSafe));
                NotifyPropertyChanged(nameof(IsFilterSafeUnlocked));
                NotifyPropertyChanged(nameof(IsFilterQuestionableUnlocked));
                NotifyPropertyChanged(nameof(IsFilterExplicitUnlocked));

                NotifyPropertyChanged(nameof(Function));
            }
        }

        public bool IsFilterQuestionable
        {
            get
            {
                return _isFilterQuestionable;
            }

            set
            {

                _isFilterQuestionable = value;
                NotifyPropertyChanged(nameof(IsFilterQuestionable));
                NotifyPropertyChanged(nameof(IsFilterSafeUnlocked));
                NotifyPropertyChanged(nameof(IsFilterQuestionableUnlocked));
                NotifyPropertyChanged(nameof(IsFilterExplicitUnlocked));

                NotifyPropertyChanged(nameof(Function));
            }
        }

        public bool IsFilterExplicit
        {
            get
            {
                return _isFilterExplicit;
            }

            set
            {

                _isFilterExplicit = value;
                NotifyPropertyChanged(nameof(IsFilterExplicit));
                NotifyPropertyChanged(nameof(IsFilterSafeUnlocked));
                NotifyPropertyChanged(nameof(IsFilterQuestionableUnlocked));
                NotifyPropertyChanged(nameof(IsFilterExplicitUnlocked));

                NotifyPropertyChanged(nameof(Function));
            }
        }


        public bool IsFilterHorizontal
        {
            get
            {
                return _isFilterHorizontal;
            }

            set
            {

                _isFilterHorizontal = value;
                NotifyPropertyChanged(nameof(IsFilterHorizontal));
                NotifyPropertyChanged(nameof(IsFilterVerticalUnlocked));
                NotifyPropertyChanged(nameof(IsFilterHorizontalUnlocked));

                NotifyPropertyChanged(nameof(Function));
            }
        }

        public bool IsFilterVertical
        {
            get
            {
                return _isFilterVertical;
            }

            set
            {
                _isFilterVertical = value;
                NotifyPropertyChanged(nameof(IsFilterVertical));
                NotifyPropertyChanged(nameof(IsFilterVerticalUnlocked));
                NotifyPropertyChanged(nameof(IsFilterHorizontalUnlocked));

                NotifyPropertyChanged(nameof(Function));
            }
        }


        public bool IsFilterAllowHidden
        {
            get
            {
                return _isFilterAllowHidden;
            }

            set
            {
                _isFilterAllowHidden = value;

                NotifyPropertyChanged(nameof(Function));
            }
        }






        public bool IsFilterSafeUnlocked
        {
            get
            {
                return !IsFilterSafe || IsFilterQuestionable || IsFilterExplicit;
            }
        }
        public bool IsFilterQuestionableUnlocked
        {
            get
            {
                return IsFilterSafe || !IsFilterQuestionable || IsFilterExplicit;
            }
        }
        public bool IsFilterExplicitUnlocked
        {
            get
            {
                return IsFilterSafe || IsFilterQuestionable || !IsFilterExplicit;
            }
        }

        // All the blacklisted tags as fucking Microsoft Store testers think sexually suggestive
        private string _tagBlacklist = String.Join(" ", new List<string>{ "swimsuits", "swimsuit", "bikini", "bikini_armor", "buruma", "ass", "pantsu", "bra", "cleavage", "underboob", "breast_hold" });

        public string TagBlacklist
        {
            get
            {
                return _tagBlacklist;
            }

            set
            {
                _tagBlacklist = value;
                NotifyPropertyChanged(nameof(TagBlacklist));

                NotifyPropertyChanged(nameof(Function));
            }
        }





        public bool IsFilterHorizontalUnlocked
        {
            get
            {
                return !IsFilterHorizontal || IsFilterVertical;
            }
        }
        public bool IsFilterVerticalUnlocked
        {
            get
            {
                return IsFilterHorizontal || !IsFilterVertical;
            }
        }

        public Func<Post, bool> Function
        {
            get
            {
                return ToFunc();
            }
        }

        public Func<Post, bool> ToFunc()
        {
            var s = IsFilterSafe;
            var q = IsFilterQuestionable;
            var e = IsFilterExplicit;

            var h = IsFilterHorizontal;
            var v = IsFilterVertical;

            var a = IsFilterAllowHidden;

            var tbl = TagBlacklist.Split(' ').ToList();

            return (o => ((o.Rating == "s" && s) || (o.Rating == "q" && q) || (o.Rating == "e" && e))
                         &&
                         ((o.Width >= o.Height && h) || (o.Width < o.Height && v))
                         &&
                         ((o.IsShownInIndex || a))
                         &&
                         (o.Tags.Split(' ').ToList().FirstOrDefault(tag => tbl.FirstOrDefault(t => String.Compare(t, tag, true) == 0) != default(string)) == default(string))
                         );
        }


    }
}
