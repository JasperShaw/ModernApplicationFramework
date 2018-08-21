using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(IDeleteActiveItemCommand))]
    internal class DeleteActiveItemCommand : CommandDefinitionCommand, IDeleteActiveItemCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            Checked = true;
            return parameter is IToolboxItem;
        }

        protected override void OnExecute(object parameter)
        {
            if (!(parameter is IToolboxItem item))
                return;
            if (ToolboxUserDialogs.AskUserForRemove(item) == MessageBoxResult.OK)
                item.Parent?.RemoveItem(item);
        }
    }
}