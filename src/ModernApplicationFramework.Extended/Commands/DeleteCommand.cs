using System.ComponentModel.Composition;
using System.Windows.Documents;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IDeleteCommand))]
    internal class DeleteCommand : CommandDefinitionCommand, IDeleteCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return EditingCommands.Delete.CanExecute(parameter, null);
        }

        protected override void OnExecute(object parameter)
        {
            EditingCommands.Delete.Execute(parameter, null);
        }
    }
}