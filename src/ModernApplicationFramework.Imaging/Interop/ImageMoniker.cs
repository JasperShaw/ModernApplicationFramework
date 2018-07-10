using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Imaging.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [Serializable]
    public struct ImageMoniker
    {
        public Guid CatalogGuid;
        public int Id;
    }
}
