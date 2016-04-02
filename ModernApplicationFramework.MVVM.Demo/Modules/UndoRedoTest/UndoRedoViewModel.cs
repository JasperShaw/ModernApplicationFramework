using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Utilities.UndoRedoManager;

namespace ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest
{
    [Export(typeof(UndoRedoViewModel))]
    public sealed class UndoRedoViewModel : Controls.Document
    {
        public override bool ShouldReopenOnStart => true;

        public IUndoRedoManager UndoRedoManager { get; }


        public UndoRedoViewModel()
        {
            PropertyChanged += UndoRedoViewModel_PropertyChanged;
            UndoRedoManager = new UndoRedoManager();
        }

        private void UndoRedoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Text))
                Debug.WriteLine("Changed");
        }

        private string _test;

        public string Text
        {
            get { return _test; }
            set
            {
                if (value == _test)
                    return;
                _test = value;
                NotifyOfPropertyChange();
            }
        }


        public ICommand SetValueCommand => new Command(SetValue);

        public ICommand UndoCommand => new Command(Undo);

        private void Undo()
        {
            Type type = GetType();

            var t = "Text";

            PropertyInfo prop = type.GetProperty(t);

            prop.SetValue(this, "", null);
        }

        private void SetValue()
        {
            Text = "5";
        }



    }
}
