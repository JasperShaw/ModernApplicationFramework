using ModernApplicationFramework.Basics.CommandBar.DataSources;

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
