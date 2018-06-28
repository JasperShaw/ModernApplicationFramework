using System;
using System.Collections.Generic;
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
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
        public override ICommand Command { get; }

        internal static CommandDefinition Instance => _instance ?? (_instance = new DeleteActiveItemCommand());

        private DeleteActiveItemCommand()
        {
            Command = new CommandEx(DeleteItem, CanDeleteItem);
        }

        private bool CanDeleteItem(object args)
        {
            return args is IToolboxItem;
        }

        private void DeleteItem(object args)
        {
            if (!(args is IToolboxItem item))
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
