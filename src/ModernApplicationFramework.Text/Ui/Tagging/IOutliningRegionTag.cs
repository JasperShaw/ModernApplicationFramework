using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public interface IOutliningRegionTag : ITag
    {
        object CollapsedForm { get; }

        object CollapsedHintForm { get; }
        bool IsDefaultCollapsed { get; }

        bool IsImplementation { get; }
    }
}