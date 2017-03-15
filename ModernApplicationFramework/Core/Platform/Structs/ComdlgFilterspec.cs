using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Platform.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal struct ComdlgFilterspec
    {
        [MarshalAs(UnmanagedType.LPWStr)] internal string pszName;
        [MarshalAs(UnmanagedType.LPWStr)] internal string pszSpec;
    }
}
