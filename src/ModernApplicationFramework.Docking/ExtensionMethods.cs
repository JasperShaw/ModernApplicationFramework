using System;

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
