using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IDeleteActiveItemCommand))]
    internal class DeleteActiveItemCommand : CommandDefinitionCommand, IDeleteActiveItemCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return parameter is IToolboxItem;
        }

        protected override void OnExecute(object parameter)
        {
            if (!(parameter is IToolboxItem item))
                return;
            if (RemoveItemDilalog.AskUserForRemove(item) == MessageBoxResult.Yes)
                item.Parent?.RemoveItem(item);
        }
    }
}