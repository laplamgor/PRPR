using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.BooruViewer.Models
{
    public abstract class PersonalizationRecord
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public int PostId { get; set; }

        public string PostPreviewUrl { get; set; }


    }

    public class WallpaperRecord : PersonalizationRecord
    {
        public static WallpaperRecord Create(Post post)
        {
            return new WallpaperRecord()
            {
                DateCreated = DateTime.UtcNow,
                PostId = post.Id,
                PostPreviewUrl = post.PreviewUrl
            };
        }
    }



    public class LockScreenRecord : PersonalizationRecord
    {
        public static LockScreenRecord Create(Post post)
        {
            return new LockScreenRecord()
            {
                DateCreated = DateTime.UtcNow,
                PostId = post.Id,
                PostPreviewUrl = post.PreviewUrl
            };
        }
    }
}
