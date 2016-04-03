using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Utilities.UndoRedoManager;

namespace ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest
{
    [Export(typeof(UndoRedoViewModel))]
    public sealed class UndoRedoViewModel : Controls.Document
    {
        public override bool ShouldReopenOnStart => true;

        private string _test;

        public UndoRedoViewModel()
        {
            DisplayName = "UndoRedoTest";
        }

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            Debug.WriteLine("Redo-Stack: " + UndoRedoManager.RedoStack.Count);
            Debug.WriteLine("Undo-Stack: " + UndoRedoManager.UndoStack.Count);
        }

        public string Text
        {
            get { return _test; }
            set
            {
                if (value == _test)
                    return;
                UndoRedoManager.Push(new UndoRedoAction(this, nameof(Text), value));
                _test = value;
                NotifyOfPropertyChange();  
            }
        }

        private string _test2;
        public string Text2
        {
            get { return _test2; }
            set
            {
                if (value == _test2)
                    return;
                UndoRedoManager.Push(new UndoRedoAction(this, nameof(Text2), value));
                _test2 = value;
                NotifyOfPropertyChange();
            }
        }

        public ICommand SetValueCommand => new Command(SetValue);

        private void SetValue()
        {
            Text += "5";
        }
    }
}
