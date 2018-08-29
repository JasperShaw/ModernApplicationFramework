using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(IFloatDockedWindowCommand))]
    internal class FloatDockedWindowCommand : CommandDefinitionCommand, IFloatDockedWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc);

            if (di?.LayoutElement.FindParent<LayoutFloatingWindow>() == null)
                return true;
            return false;
        }

        protected override void OnExecute(object parameter)
        {
            var dc = DockingManager.Instance?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instance?.GetLayoutItemFromModel(dc);
            di?.FloatCommand.Execute(null);
        }
    }
}