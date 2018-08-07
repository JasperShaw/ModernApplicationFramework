using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface IFontAndColorDefaultsProvider
    {
        object GetObject(ref Guid guidCategory);
    }
}