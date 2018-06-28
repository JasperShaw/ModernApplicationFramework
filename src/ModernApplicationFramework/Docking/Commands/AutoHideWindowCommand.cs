using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IAutoHideWindowCommand))]
    internal class AutoHideWindowCommand : CommandDefinitionCommand, IAutoHideWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            if (DockingManager.Instance == null)
                return false;
            var dc = DockingManager.Instance.Layout.ActiveContent;
            if (dc == null)
                return false;

            var di = DockingManager.Instance.GetLayoutItemFromModel(dc);

            if (di?.LayoutElement == null)
                return false;

            if (di.LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null)
                return false;

            return di.LayoutElement is LayoutAnchorable layoutItem && layoutItem.CanAutoHide &&
                   !layoutItem.IsAutoHidden;
        }

        protected override void OnExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return;

            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc) as LayoutAnchorableItem;

            di?.AutoHideCommand.Execute(null);
        }
    }
}