using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Extended.Core
{
    [Export(typeof(CommandBarUndoRedoManagerWatcher))]
    public class CommandBarUndoRedoManagerWatcher
    {
        private readonly IDockingHostViewModel _shell;
        private IUndoRedoManager _undoRedoManager;

        public IObservableCollection<IHasTextProperty> RedoItems { get; set; }
        public IObservableCollection<IHasTextProperty> UndoItems { get; set; }

        public IUndoRedoManager UndoRedoManager
        {
            get => _undoRedoManager;
            set
            {
                if (_undoRedoManager != null)
                {
                    _undoRedoManager.UndoStack.CollectionChanged -= OnUndoStackChanged;
                    _undoRedoManager.RedoStack.CollectionChanged -= OnRedoStackChanged;
                    _undoRedoManager.ChangingBegin -= _undoRedoManager_ChangingBegin;
                    _undoRedoManager.ChangingEnd -= _undoRedoManager_ChangingEnd;
                }
                _undoRedoManager = value;
                if (_undoRedoManager != null)
                {
                    _undoRedoManager.UndoStack.CollectionChanged += OnUndoStackChanged;
                    _undoRedoManager.RedoStack.CollectionChanged += OnRedoStackChanged;
                    _undoRedoManager.ChangingBegin += _undoRedoManager_ChangingBegin; ;
                    _undoRedoManager.ChangingEnd += _undoRedoManager_ChangingEnd; ;
                }
            }
        }

        private bool _isRunning;

        private void _undoRedoManager_ChangingEnd(object sender, EventArgs e)
        {
            _isRunning = false;
            Refresh();

        }

        private void _undoRedoManager_ChangingBegin(object sender, EventArgs e)
        {
            _isRunning = true;
        }

        [ImportingConstructor]
        public CommandBarUndoRedoManagerWatcher(IDockingHostViewModel shell)
        {
            if (shell == null)
                return;
            _shell = shell;

            UndoItems = new BindableCollection<IHasTextProperty>();
            RedoItems = new BindableCollection<IHasTextProperty>();
            shell.ActiveDocumentChanged += Shell_ActiveDocumentChanged;
            UndoRedoManager = shell.ActiveItem?.UndoRedoManager;
        }

        public void Refresh()
        {
            if (_isRunning)
                return;

            UndoItems.Clear();
            if (_undoRedoManager == null)
                return;
            foreach (var t in _undoRedoManager.UndoStack.Reverse())
                UndoItems.Add(new TextCommandBarItemDefinition(t.Description));

            RedoItems.Clear();
            if (_undoRedoManager == null)
                return;
            foreach (var t in _undoRedoManager.RedoStack.Reverse())
                RedoItems.Add(new TextCommandBarItemDefinition(t.Description));
        }

        private void OnUndoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        private void OnRedoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        private void Shell_ActiveDocumentChanged(object sender, EventArgs e)
        {
            UndoRedoManager = _shell.ActiveItem?.UndoRedoManager;
        }
    }
}