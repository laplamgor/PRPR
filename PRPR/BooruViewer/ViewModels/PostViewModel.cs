using PRPR.BooruViewer.Models;
using PRPR.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.ViewModels
{
    public class PostViewModel : IImageWallItemImage
    {
        public readonly Post Post;


        public PostViewModel(Post post)
        {
            this.Post = post;
        }
        


        public double PreferredWidth
        {
            get
            {
                return this.Post.Width;
            }
        }

        public double PreferredHeight
        {
            get
            {
                return Post.Height;
            }
        }

        public double PreferredRatio
        {
            get
            {
                return (double)Post.Width / Post.Height;
            }
        }
    }
}
