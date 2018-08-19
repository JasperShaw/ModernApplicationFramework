using System;

namespace ModernApplicationFramework.Editor.Interop
{
    public interface IFontAndColorGroup
    {
        Guid GroupGuid { get; }

        int GetCount();

        ushort GetPriority();

        string GetGroupName();

        Guid GetCategory(int category);
    }
}