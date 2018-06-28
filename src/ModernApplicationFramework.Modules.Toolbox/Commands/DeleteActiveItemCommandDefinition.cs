using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    internal class DeleteActiveItemCommandDefinition : CommandDefinition<IDeleteActiveItemCommand>
    {
        private static DeleteActiveItemCommandDefinition _instance;
        public override string NameUnlocalized => null;
        public override string Text => null;
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => null;
        public override Guid Id => Guid.Empty;
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
        internal static CommandDefinition Instance => _instance ?? (_instance = new DeleteActiveItemCommandDefinition());
    }

    internal interface IDeleteActiveItemCommand : ICommandDefinitionCommand
    {
    }

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
