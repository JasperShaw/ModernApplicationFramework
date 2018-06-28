using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.WindowManagement.Interfaces.Commands;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(ISaveCurrentLayoutCommand))]
    internal class SaveCurrentLayoutCommand : CommandDefinitionCommand, ISaveCurrentLayoutCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return LayoutManagementService.Instance != null;
        }

        protected override void OnExecute(object parameter)
        {
            LayoutManagementService.Instance.LayoutManager.SaveWindowLayout();
        }
    }
}