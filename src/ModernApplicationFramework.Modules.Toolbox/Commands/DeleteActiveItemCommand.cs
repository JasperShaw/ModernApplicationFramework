using System;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    internal class DeleteActiveItemCommand : CommandDefinition
    {
        private static DeleteActiveItemCommand _instance;
        public override string NameUnlocalized => null;
        public override string Text => null;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => null;
        public override Guid Id => Guid.Empty;
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;
        public override ICommand Command { get; }

        internal static CommandDefinition Instance => _instance ?? (_instance = new DeleteActiveItemCommand());

        public DeleteActiveItemCommand()
        {
            Command = new Command(DeleteItem, CanDeleteItem);
        }

        private bool CanDeleteItem()
        {
            return CommandParamenter is IToolboxItem;
        }

        private void DeleteItem()
        {
            if (!(CommandParamenter is IToolboxItem item))
                return;
            if (AskUserForRemove(item) == MessageBoxResult.Yes)
                item.Parent?.RemoveItem(item);
        }

        private MessageBoxResult AskUserForRemove(IToolboxItem item)
        {
            IoC.Get<IMafUIShell>().GetAppName(out var name);
            return MessageBox.Show("Delete", name, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        }
    }
}
