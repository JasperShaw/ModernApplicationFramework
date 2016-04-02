using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Input;
using ModernApplicationFramework.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest
{
    [Export(typeof(UndoRedoViewModel))]
    public sealed class UndoRedoViewModel : Controls.Document
    {
        public override bool ShouldReopenOnStart => true;


        public UndoRedoViewModel()
        {
            PropertyChanged += UndoRedoViewModel_PropertyChanged;
        }

        private void UndoRedoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
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
