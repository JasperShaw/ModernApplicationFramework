using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.Commands
{
    [Export(typeof(ICloseDockedWindowCommand))]
    internal class CloseDockedWindowCommand : CommandDefinitionCommand, ICloseDockedWindowCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            //TODO
            Visible = dm != null;
            Enabled = dm != null;

            return dm != null;
        }

        protected override void OnExecute(object parameter)
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            var item = DockingManager.Instance?.GetLayoutItemFromModel(dm);
            item?.CloseCommand.Execute(null);
        }
    }
}