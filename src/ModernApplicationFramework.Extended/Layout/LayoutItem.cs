using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class LayoutItem : LayoutItemBase, ILayoutItem
    {

        public override void TryClose(bool? dialogResult = null)
        {
            DockingModel.CloseCommand.Execute(null);
        }
    }
}