using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IProvideColorableItems
    {
        Guid LanguageId { get; }

        int GetItemCount(out int count);

        int GetColorableItem(int index, out IColorableItem item);
    }
}