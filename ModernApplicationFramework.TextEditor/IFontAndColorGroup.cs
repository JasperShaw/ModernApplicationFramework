using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface IFontAndColorGroup
    {
        int GetCount();

        FcPriority GetPriority();

        string GetGroupName();

        Guid GetCategory(int category);
    }
}