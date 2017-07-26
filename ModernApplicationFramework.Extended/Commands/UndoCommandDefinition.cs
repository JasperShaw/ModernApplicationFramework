using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Core;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(UndoCommandDefinition))]
    public sealed class UndoCommandDefinition : CommandDefinition
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override CommandGestureCategory DefaultGestureCategory { get; }

        public override string IconId => "UndoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Undo_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Text => Commands_Resources.UndoCommandDefinition_Text;
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;

        [ImportingConstructor]
        public UndoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var command = new UICommand(Undo, CanUndo);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(new[] { Key.Z }, ModifierKeys.Control);
            DefaultGestureCategory = CommandGestureCategories.GlobalGestureCategory;

            _watcher = watcher;
        }

        private bool CanUndo()
        {
            return _watcher?.UndoRedoManager != null && _watcher.UndoRedoManager.UndoStack.Any();
        }

        private void Undo()
        {
            _watcher.UndoRedoManager.Undo(1);
        }
    }
}