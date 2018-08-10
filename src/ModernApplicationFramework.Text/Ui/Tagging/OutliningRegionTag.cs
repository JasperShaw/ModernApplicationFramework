namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public class OutliningRegionTag : IOutliningRegionTag
    {
        public object CollapsedForm { get; }

        public object CollapsedHintForm { get; }
        public bool IsDefaultCollapsed { get; }

        public bool IsImplementation { get; }

        public OutliningRegionTag()
            : this(false, false, null, null)
        {
        }

        public OutliningRegionTag(object collapsedForm, object collapsedHintForm)
            : this(false, false, collapsedForm, collapsedHintForm)
        {
        }

        public OutliningRegionTag(bool isDefaultCollapsed, bool isImplementation, object collapsedForm,
            object collapsedHintForm)
        {
            IsDefaultCollapsed = isDefaultCollapsed;
            IsImplementation = isImplementation;
            CollapsedForm = collapsedForm;
            CollapsedHintForm = collapsedHintForm;
        }
    }
}