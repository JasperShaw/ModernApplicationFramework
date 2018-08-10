using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Text.Ui.Outlining
{
    public interface ICollapsible
    {
        object CollapsedForm { get; }

        object CollapsedHintForm { get; }
        ITrackingSpan Extent { get; }

        bool IsCollapsed { get; }

        bool IsCollapsible { get; }

        IOutliningRegionTag Tag { get; }
    }
}