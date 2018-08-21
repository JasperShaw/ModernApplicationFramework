using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    [Export(typeof(ICustomizeMenuCommand))]
    internal class CustomizeMenuCommand : CommandDefinitionCommand, ICustomizeMenuCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var windowManager = new WindowManager();
            var customizeDialog = IoC.Get<CustomizeDialogViewModel>();
            windowManager.ShowDialog(customizeDialog);
        }
    }
}