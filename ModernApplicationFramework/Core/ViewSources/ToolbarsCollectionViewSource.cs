using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.ViewSources
{
    public class ToolbarsCollectionViewSource : PropertyBoundFilteringCollectionViewSource<ToolbarDefinition>
    {
        protected override bool AcceptItem(ToolbarDefinition item)
        {
            return true;
        }
    }
}
