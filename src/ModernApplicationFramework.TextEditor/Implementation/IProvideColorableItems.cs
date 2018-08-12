using System;

namespace ModernApplicationFramework.Editor.Implementation
{
    public interface IProvideColorableItems
    {
        Guid LanguageId { get; }

        int GetItemCount(out int count);

        int GetColorableItem(int index, out IColorableItem item);
    }
}