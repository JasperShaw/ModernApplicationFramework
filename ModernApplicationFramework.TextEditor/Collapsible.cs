namespace ModernApplicationFramework.TextEditor
{
    internal class Collapsible : ICollapsible
    {
        public ITrackingSpan Extent { get; }

        public bool IsCollapsed { get; internal set; }

        public object CollapsedForm => Tag?.CollapsedForm;

        public object CollapsedHintForm => Tag?.CollapsedHintForm;

        public IOutliningRegionTag Tag { get; internal set; }

        public bool IsCollapsible => true;

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

        public Collapsible(ITrackingSpan underlyingSpan, IOutliningRegionTag tag)
        {
            Extent = underlyingSpan;
            Tag = tag;
            IsCollapsed = false;
        }
    }
}