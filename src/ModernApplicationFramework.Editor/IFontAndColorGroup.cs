using System;

namespace ModernApplicationFramework.Editor
{
    public interface IFontAndColorGroup
    {
        int GetCount();

        FcPriority GetPriority();

        string GetGroupName();

        Guid GetCategory(int category);
    }
}