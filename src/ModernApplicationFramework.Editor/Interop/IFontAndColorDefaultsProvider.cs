using System;

namespace ModernApplicationFramework.Editor.Interop
{
    public interface IFontAndColorDefaultsProvider
    {
        object GetObject(ref Guid guidCategory);
    }
}