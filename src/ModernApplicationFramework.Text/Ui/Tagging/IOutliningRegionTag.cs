using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public interface IOutliningRegionTag : ITag
    {
        bool IsDefaultCollapsed { get; }

        bool IsImplementation { get; }

        object CollapsedForm { get; }

        object CollapsedHintForm { get; }
    }
}