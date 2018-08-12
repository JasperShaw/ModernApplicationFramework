using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Olecmd
    {
        public uint cmdID;
        public Olecmdf cmdf;
    }
}