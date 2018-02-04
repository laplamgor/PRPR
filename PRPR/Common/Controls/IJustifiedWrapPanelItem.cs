using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.Common.Controls
{
    public interface IJustifiedWrapPanelItem
    {
        double PreferredWidth
        {
            get;
        }

        double PreferredHeight
        {
            get;
        }


        double PreferredRatio
        {
            get;
        }
    }
}
