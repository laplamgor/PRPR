using PRPR.BooruViewer.Models;
using PRPR.BooruViewer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private Post _post = null;

        public Post Post
        {
            get
            {
                return _post;
            }

            set
            {
                _post = value;
                NotifyPropertyChanged(nameof(Post));
            }
        }


        public bool IsFavorited
        {
            get
            {
                return _isFavorited;
            }

            private set
            {
                _isFavorited = value;
                NotifyPropertyChanged(nameof(IsFavorited));
            }
        }


        private Comments _comments = null;


        public Comments Comments
        {
            get
            {
                return _comments;
            }

            set
            {
                _comments = value;
                NotifyPropertyChanged(nameof(Comments));
            }
        }

        private bool _isFavorited = false;


        public async Task UpdateIsFavorited()
        {
            IsFavorited = await YandeClient.CheckFavorited(Post.Id);
        }

        public async Task Favorite()
        {
            await YandeClient.AddFavoriteAsync(Post.Id);
            this.IsFavorited = true;

        }
        public async Task Unfavorite()
        {
            await YandeClient.RemoveFavoriteAsync(Post.Id);
            this.IsFavorited = false;
        }
    }
}
