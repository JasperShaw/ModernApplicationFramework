using System;

namespace ModernApplicationFramework.Editor.Interop
{
    public interface IFontAndColorGroup
    {
        int GetCount();

        FcPriority GetPriority();

        string GetGroupName();

        Guid GetCategory(int category);
    }
}