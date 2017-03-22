using ModernApplicationFramework.Basics.Definitions;

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
