using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Native.Platform.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct ComdlgFilterspec
    {
        [MarshalAs(UnmanagedType.LPWStr)] internal string pszName;
        [MarshalAs(UnmanagedType.LPWStr)] internal string pszSpec;
    }
}
