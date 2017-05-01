using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(MultiUndoCommandDefinition))]
    public sealed class MultiUndoCommandDefinition : CommandSplitButtonDefinition
    {
        private readonly IDockingHostViewModel _shell;
        private IUndoRedoManager _undoRedoManager;


        private IUndoRedoManager UndoRedoManager
        {
            set
            {
                if (_undoRedoManager != null)
                    _undoRedoManager.UndoStack.CollectionChanged -= OnUndoRedoStackChanged;
                _undoRedoManager = value;
                if (_undoRedoManager != null)
                    _undoRedoManager.UndoStack.CollectionChanged += OnUndoRedoStackChanged;
            }
        }

        private void OnUndoRedoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Items.Clear();
            if (_undoRedoManager != null)
            {
                foreach (var t in _undoRedoManager.UndoStack.Reverse())
                    Items.Add(new TestItem(t.Name));
            }
        }

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
        public MultiUndoCommandDefinition(IDockingHostViewModel shell)
        {
            var command = new MultiKeyGestureCommandWrapper(Undo, CanUndo,
                new MultiKeyGesture(new[] {Key.Z}, ModifierKeys.Control));
            Command = command;
            ShortcutText = command.GestureText;

            Items = new BindableCollection<object>();

            _shell = shell;
            if (shell == null)
                return;
            _shell.ActiveDocumentChanged += DockingHost_ActiveDocumentChanged;
            UndoRedoManager = _shell.ActiveItem?.UndoRedoManager;
        }

        private void DockingHost_ActiveDocumentChanged(object sender, EventArgs e)
        {
            UndoRedoManager = _shell.ActiveItem?.UndoRedoManager;
        }

        private bool CanUndo()
        {
            if (_undoRedoManager == null)
                return false;
            return _undoRedoManager.UndoStack.Any();
        }

        private void Undo()
        {
            _undoRedoManager.Undo();
        }

        public override IObservableCollection<object> Items { get; set; }
        public override void Execute(int count)
        {
            _undoRedoManager?.Undo(count);
        }
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