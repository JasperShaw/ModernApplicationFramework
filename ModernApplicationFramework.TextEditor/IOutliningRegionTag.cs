namespace ModernApplicationFramework.TextEditor
{
    public interface IOutliningRegionTag : ITag
    {
        bool IsDefaultCollapsed { get; }

        bool IsImplementation { get; }

        object CollapsedForm { get; }

        object CollapsedHintForm { get; }
    }
}