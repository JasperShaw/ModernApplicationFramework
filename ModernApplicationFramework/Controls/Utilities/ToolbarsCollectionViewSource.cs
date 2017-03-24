using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Controls.Utilities
{
    public class ToolbarsCollectionViewSource : PropertyBoundFilteringCollectionViewSource<ToolbarDefinitionOld>
    {
        protected override bool AcceptItem(ToolbarDefinitionOld item)
        {
            return true;
        }
    }
}
