using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Extended.Core;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MultiUndoCommandDefinition))]
    public sealed class MultiUndoCommandDefinition : CommandSplitButtonDefinition
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

        public override string Name => Commands_Resources.MultiUndoCommandDefinition_Name;
        public override string Text => Commands_Resources.MultiUndoCommandDefinition_Text;
        public override string ToolTip => Commands_Resources.MultiUndoCommandDefinition_ToolTip;

        public override CommandCategory Category => CommandCategories.EditCommandCategory;

        [ImportingConstructor]
        public MultiUndoCommandDefinition(CommandBarUndoRedoManagerWatcher watcher)
        {
            var command = new UICommand(Undo, CanUndo);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(new[] { Key.Z }, ModifierKeys.Control);
            DefaultGestureCategory = CommandGestureCategories.GlobalGestureCategory;

            _watcher = watcher;
            Items = _watcher.UndoItems;
        }

        private bool CanUndo()
        {
            return _watcher.UndoRedoManager != null && _watcher.UndoRedoManager.UndoStack.Any();
        }

        private void Undo()
        {
            _watcher.UndoRedoManager.Undo(1);
        }

        public override IObservableCollection<IHasTextProperty> Items { get; set; }
        public override void Execute(int count)
        {
            _watcher.UndoRedoManager?.Undo(count);
        }
    }
}