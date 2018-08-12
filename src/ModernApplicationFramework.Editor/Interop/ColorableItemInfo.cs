using System.Runtime.InteropServices;
using System.Windows.Media;
using ModernApplicationFramework.Editor.TextManager;

namespace ModernApplicationFramework.Editor.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ColorableItemInfo
    {
        public Color Foreground;
        public Color Background;
        public FontFlags FontFlags;
    }
}