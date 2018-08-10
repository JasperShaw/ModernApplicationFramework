using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Text.Ui.Outlining
{
    public interface ICollapsible
    {
        ITrackingSpan Extent { get; }

        bool IsCollapsed { get; }

        bool IsCollapsible { get; }

        object CollapsedForm { get; }

        object CollapsedHintForm { get; }

        IOutliningRegionTag Tag { get; }
    }
}