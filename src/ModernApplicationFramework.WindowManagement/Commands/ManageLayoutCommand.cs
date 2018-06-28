using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.WindowManagement.Interfaces.Commands;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(IManageLayoutCommand))]
    internal class ManageLayoutCommand : CommandDefinitionCommand, IManageLayoutCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            if (LayoutManagementService.Instance == null)
                return false;
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            LayoutManagementService.Instance.LayoutManager.ManageWindowLayouts();
        }
    }
}