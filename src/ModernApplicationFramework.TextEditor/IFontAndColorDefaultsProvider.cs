using System;

namespace ModernApplicationFramework.Editor
{
    public interface IFontAndColorDefaultsProvider
    {
        object GetObject(ref Guid guidCategory);
    }
}