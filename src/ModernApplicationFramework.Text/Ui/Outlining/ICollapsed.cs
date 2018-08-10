using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Ui.Outlining
{
    public interface ICollapsed : ICollapsible
    {
        IEnumerable<ICollapsed> CollapsedChildren { get; }
    }
}