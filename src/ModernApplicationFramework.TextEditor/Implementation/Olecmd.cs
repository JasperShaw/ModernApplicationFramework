using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Olecmd
    {
        public uint cmdID;
        public uint cmdf;
    }
}