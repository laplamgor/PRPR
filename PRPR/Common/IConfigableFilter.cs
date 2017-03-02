using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.Common
{
    public interface IConfigableFilter<T> : INotifyPropertyChanged
    {
        Func<T, bool> Function { get; }
    }
}
