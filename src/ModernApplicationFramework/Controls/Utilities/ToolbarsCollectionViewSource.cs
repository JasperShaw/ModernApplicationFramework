using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Controls.Utilities
{
    public class ToolbarsCollectionViewSource : PropertyBoundFilteringCollectionViewSource<ToolbarDefinition>
    {
        protected override bool AcceptItem(ToolbarDefinition item)
        {
            return true;
        }
    }
}
