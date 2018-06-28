using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Dialogs.WindowSelectionDialog;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(IWindowSelectCommand))]
    internal class WindowSelectCommand : CommandDefinitionCommand, IWindowSelectCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            IoC.Get<IWindowManager>().ShowDialog(IoC.Get<IWindowSelectionDialogViewModel>());
        }
    }
}