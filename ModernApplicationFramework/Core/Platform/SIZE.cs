using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Platform
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    // ReSharper disable once InconsistentNaming
    public struct SIZE
    {
        [ComAliasName("Microsoft.VisualStudio.OLE.Interop.LONG")] public int cx;
        [ComAliasName("Microsoft.VisualStudio.OLE.Interop.LONG")] public int cy;
    }
}