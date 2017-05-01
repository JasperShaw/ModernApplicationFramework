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

        public IObservableCollection<IHasTextProperty> Items { get; set; }

        public IUndoRedoManager UndoRedoManager
        {
            get => _undoRedoManager;
            set
            {
                if (_undoRedoManager != null)
                    _undoRedoManager.UndoStack.CollectionChanged -= OnUndoRedoStackChanged;
                _undoRedoManager = value;
                if (_undoRedoManager != null)
                    _undoRedoManager.UndoStack.CollectionChanged += OnUndoRedoStackChanged;
            }
        }

        [ImportingConstructor]
        public CommandBarUndoRedoManagerWatcher(IDockingHostViewModel shell)
        {
            if (shell == null)
                return;
            _shell = shell;

            Items = new BindableCollection<IHasTextProperty>();

            shell.ActiveDocumentChanged += Shell_ActiveDocumentChanged;
            ;
            UndoRedoManager = shell.ActiveItem?.UndoRedoManager;
        }

        private void OnUndoRedoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Items.Clear();
            if (_undoRedoManager != null)
                foreach (var t in _undoRedoManager.UndoStack.Reverse())
                    Items.Add(new TextCommandBarItemDefinition(t.Name));
        }

        private void Shell_ActiveDocumentChanged(object sender, EventArgs e)
        {
            UndoRedoManager = _shell.ActiveItem?.UndoRedoManager;
        }
    }
}