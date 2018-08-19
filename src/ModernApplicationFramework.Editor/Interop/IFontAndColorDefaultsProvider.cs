using System;

namespace ModernApplicationFramework.Editor.Interop
{
    public interface IFontAndColorDefaultsProvider
    {
        object GetObject(Guid guidCategory);
    }
}