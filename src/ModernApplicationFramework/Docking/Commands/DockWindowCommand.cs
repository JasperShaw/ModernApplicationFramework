using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IDockWindowCommand))]
    internal class DockWindowCommand : CommandDefinitionCommand, IDockWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc);

            return di?.LayoutElement?.FindParent<LayoutFloatingWindow>() != null ||
                   di?.LayoutElement?.FindParent<LayoutDocumentPane>() != null && di.LayoutElement is LayoutAnchorable ||
                   di?.LayoutElement is LayoutAnchorable layoutItem && layoutItem.IsAutoHidden;
        }

        protected override void OnExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc);

            if (di is LayoutAnchorableItem anchorableItem)
                anchorableItem.DockCommand.Execute(null);
            else
                di.DockAsDocumentCommand.Execute(null);
        }
    }
}