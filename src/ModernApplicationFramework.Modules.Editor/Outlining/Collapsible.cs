using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.Outlining
{
    internal class Collapsible : ICollapsible
    {
        public object CollapsedForm => Tag?.CollapsedForm;

        public object CollapsedHintForm => Tag?.CollapsedHintForm;
        public ITrackingSpan Extent { get; }

        public bool IsCollapsed { get; internal set; }

        public bool IsCollapsible => true;

        public IOutliningRegionTag Tag { get; internal set; }

        public Collapsible(ITrackingSpan underlyingSpan, IOutliningRegionTag tag)
        {
            Extent = underlyingSpan;
            Tag = tag;
            IsCollapsed = false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Collapsible collapsible && Tag.Equals(collapsible.Tag) && Extent.Equals(collapsible.Extent))
                return IsCollapsed == collapsible.IsCollapsed;
            return false;
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode() ^ Extent.GetHashCode() ^ IsCollapsed.GetHashCode();
        }
    }
}