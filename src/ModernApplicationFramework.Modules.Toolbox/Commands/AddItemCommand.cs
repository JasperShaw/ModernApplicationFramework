using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IAddItemCommand))]
    internal class AddItemCommand : CommandDefinitionCommand, IAddItemCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            IoC.Get<IWindowManager>().ShowDialog(IoC.Get<ChooseItemsDialog.ChooseItemsDialogViewModel>());
        }
    }
}