using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(MultiUndoCommandDefinition))]
    public sealed class MultiUndoCommandDefinition : CommandSplitButtonDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649
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

        public MultiUndoCommandDefinition()
        {
            var command = new MultiKeyGestureCommandWrapper(Undo, CanUndo,
                new MultiKeyGesture(new[] {Key.Z}, ModifierKeys.Control));
            Command = command;
            ShortcutText = command.GestureText;

            Items = new ObservableCollection<object>
            {
                new TestItem("Test"), new TestItem("Test2")
            };
        }

        private bool CanUndo()
        {
            return _shell?.DockingHost.ActiveItem != null &&
                   _shell.DockingHost.ActiveItem.UndoRedoManager.UndoStack.Any();
        }

        private void Undo()
        {
            _shell.DockingHost.ActiveItem.UndoCommand.Execute(null);
        }

        public override ObservableCollection<object> Items { get; set; }
    }

    public class TestItem
    {
        public TestItem(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}