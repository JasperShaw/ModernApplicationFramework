using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.UndoRedoManager
{
    [Export(typeof(CommandBarUndoRedoManagerWatcher))]
    public class CommandBarUndoRedoManagerWatcher
    {
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
            UndoItems = new BindableCollection<IHasTextProperty>();
            RedoItems = new BindableCollection<IHasTextProperty>();
            //shell.ActiveLayoutItemChanged += ShellActiveLayoutItemChanged;
            shell.ActiveModelChanged += ShellOnActiveModelChanged;
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
                UndoItems.Add(new TextDataModel(t.Description));

            RedoItems.Clear();
            if (_undoRedoManager == null)
                return;
            foreach (var t in _undoRedoManager.RedoStack.Reverse())
                RedoItems.Add(new TextDataModel(t.Description));
        }

        private void OnUndoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        private void OnRedoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }


        private void ShellOnActiveModelChanged(object sender, LayoutBaselChangeEventArgs e)
        {
            UndoRedoManager = e.NewLayoutItem?.UndoRedoManager;
        }
    }
}