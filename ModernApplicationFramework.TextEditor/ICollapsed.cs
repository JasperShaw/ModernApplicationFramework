using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ICollapsed : ICollapsible
    {
        IEnumerable<ICollapsed> CollapsedChildren { get; }
    }
}