using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal interface ICategorizedFontColorPrefs
    {
        //TODO Implement interface
        int GetFontColorPreferences([In] ref Guid rguidFontCategory, [In] ref Guid rguidColorCategory, [In, Out] ref FontColorPreferences2 pPrefs);
    }
}