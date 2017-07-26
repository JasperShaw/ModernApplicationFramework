using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Core;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MultiRedoCommandDefinition))]
    public sealed class MultiRedoCommandDefinition : CommandSplitButtonDefinition
    {
        private readonly CommandBarUndoRedoManagerWatcher _watcher;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override CommandGestureCategory DefaultGestureCategory { get; }

        public override string IconId => "RedoIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Redo_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => Commands_Resources.MultiRedoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiRedoCommandDefinition_Text;
        public override string ToolTip => Commands_Resources.MultiRedoCommandDefinition_ToolTip;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;

        [ImportingConstructor]
        public MultiRedoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var command = new UICommand(Redo, CanRedo);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(new[] { Key.Y }, ModifierKeys.Control);
            DefaultGestureCategory = CommandGestureCategories.GlobalGestureCategory;

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