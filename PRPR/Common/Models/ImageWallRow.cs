using System.Collections.ObjectModel;
using System.Linq;

namespace PRPR.Common.Models
{
    public class ImageWallRow<T> : ObservableCollection<ImageWallItem<T>> where T : IImageWallItemImage
    {
        public double MaxRowWidth { get; set; } = 0;

        public double MinWidth
        {
            get
            {
                return 0;
            }
        }

        public double RowHeight { get; set; } = 0;

        public double RowWidth
        {
            get
            {
                return this.Sum(o => o.DisplayWidth);
            }
        }
    }


}
