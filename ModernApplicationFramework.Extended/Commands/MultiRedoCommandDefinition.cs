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
    [Export(typeof(MultiRedoCommandDefinition))]
    public sealed class MultiRedoCommandDefinition : CommandSplitButtonDefinition
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        public override ICommand Command { get; }

        public override string IconId => "RedoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Redo_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Edit.Redo";
        public override string Text => "Redo";
        public override string ToolTip => "Redo";

        public override CommandCategory Category => CommandCategories.EditCommandCategory;

        [ImportingConstructor]
        public MultiRedoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var command = new MultiKeyGestureCommandWrapper(Redo, CanRedo,
                new MultiKeyGesture(new[] {Key.Y}, ModifierKeys.Control));
            Command = command;
            ShortcutText = command.GestureText;
            _watcher = watcher;
            Items = _watcher.RedoItems;
        }

        private bool CanRedo()
        {
            return _watcher.UndoRedoManager != null && _watcher.UndoRedoManager.RedoStack.Any();
        }

        private void Redo()
        {
            _watcher.UndoRedoManager.Redo(1);
        }

        public override IObservableCollection<IHasTextProperty> Items { get; set; }
        public override void Execute(int count)
        {
            _watcher.UndoRedoManager?.Redo(count);
        }
    }
}