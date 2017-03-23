using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Extended.TestWindow.UndoRedoTest
{
    [DisplayName("UndoRedoTest")]
    [Export(typeof(UndoRedoViewModel))]
    public sealed class UndoRedoViewModel : Core.LayoutItems.LayoutItem
    {
        public override bool ShouldReopenOnStart => true;

        private string _test;

        public UndoRedoViewModel()
        {
            DisplayName = "UndoRedoTest";
        }

        [DisplayName("Text"), Description("Nothing special"), Category("Text")]
        public string Text
        {
            get => _test;
            set
            {
                if (value == _test)
                    return;
                PushUndoRedoManager(nameof(Text), value);
                _test = value;
                NotifyOfPropertyChange();  
            }
        }

        private string _test2;

        [DisplayName("Text 2"), Description("Nothing special"), Category("Text")]
        public string Text2
        {
            get => _test2;
            set
            {
                if (value == _test2)
                    return;
                PushUndoRedoManager(nameof(Text2), value);
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
