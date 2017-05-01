using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Core;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(MultiUndoCommandDefinition))]
    public sealed class MultiUndoCommandDefinition : CommandSplitButtonDefinition
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;


        public override ICommand Command { get; }

        public override string IconId => "UndoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Undo_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Edit.Undo";
        public override string Text => "Undo";
        public override string ToolTip => "Undo";

        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public string MyText { get; set; }

        [ImportingConstructor]
        public MultiUndoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var command = new MultiKeyGestureCommandWrapper(Undo, CanUndo,
                new MultiKeyGesture(new[] {Key.Z}, ModifierKeys.Control));
            Command = command;
            ShortcutText = command.GestureText;
            _watcher = watcher;
            Items = _watcher.Items;
        }

        private bool CanUndo()
        {
            return _watcher.UndoRedoManager != null && _watcher.UndoRedoManager.UndoStack.Any();
        }

        private void Undo()
        {
            _watcher.UndoRedoManager.Undo();
        }

        public override IObservableCollection<IHasTextProperty> Items { get; set; }
        public override void Execute(int count)
        {
            _watcher.UndoRedoManager?.Undo(count);
        }
    }
}