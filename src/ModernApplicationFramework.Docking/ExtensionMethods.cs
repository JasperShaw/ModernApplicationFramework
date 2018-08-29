using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Docking
{
    internal static class ExtensionMethods
    {
        internal static TV GetValueOrDefault<TV>(this WeakReference wr)
        {
            if (wr == null || !wr.IsAlive)
                return default;
            return (TV)wr.Target;
        }
    }
}
