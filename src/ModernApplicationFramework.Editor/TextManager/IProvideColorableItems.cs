using System;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor.TextManager
{
    public interface IProvideColorableItems
    {
        Guid LanguageId { get; }

        int GetItemCount(out int count);

        int GetColorableItem(int index, out IColorableItem item);
    }
}