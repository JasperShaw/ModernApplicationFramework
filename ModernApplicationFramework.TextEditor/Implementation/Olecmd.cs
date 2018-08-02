using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Olecmd
    {
        public uint cmdID;
        public uint cmdf;
    }
}