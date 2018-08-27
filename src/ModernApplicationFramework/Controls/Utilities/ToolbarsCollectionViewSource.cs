using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Controls.Utilities
{
    internal class ToolbarsCollectionViewSource : PropertyBoundFilteringCollectionViewSource<ToolBarDataSource>
    {
        protected override bool AcceptItem(ToolBarDataSource item)
        {
            return true;
        }
    }
}
