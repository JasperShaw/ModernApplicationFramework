using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IHideDockedWindowCommand))]
    internal class HideDockedWindowCommand : CommandDefinitionCommand, IHideDockedWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            return dm is LayoutAnchorable;
        }

        protected override void OnExecute(object parameter)
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            var item = DockingManager.Instance?.GetLayoutItemFromModel(dm) as LayoutAnchorableItem;
            item?.HideCommand.Execute(null);
        }
    }
}