using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public class ToolbarItemGroupDefinition : CommandBarDefinitionBase
    {
        public ToolbarDefinition ParentToolbar { get; set; }

        public ToolbarItemGroupDefinition(ToolbarDefinition toolbar, uint sortOrder) : base(null, sortOrder, null, false, false)
        {
            ParentToolbar = toolbar;
        }
    }
}