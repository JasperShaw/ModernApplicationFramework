using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Extended.UndoRedoManager;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(UndoCommandDefinition))]
    public sealed class UndoCommandDefinition : CommandDefinition<IUndoCommand>
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "UndoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Undo_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Text => Commands_Resources.UndoCommandDefinition_Text;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("UndoCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{1A236C59-DA8D-424F-804B-22D80CFA15D6}");

        public UndoCommandDefinition()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.Z, ModifierKeys.Control)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface IUndoCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IUndoCommand))]
    internal class UndoCommand : CommandDefinitionCommand, IUndoCommand
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        [ImportingConstructor]
        public UndoCommand(CommandBarUndoRedoManagerWatcher watcher)
        {
            _watcher = watcher;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _watcher?.UndoRedoManager != null && _watcher.UndoRedoManager.UndoStack.Any();
        }

        protected override void OnExecute(object parameter)
        {
            _watcher.UndoRedoManager.Undo(1);
        }
    }
}