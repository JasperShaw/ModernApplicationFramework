using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Modules.OutputTool;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.UndoRedoTest
{
    [DisplayName("UndoRedoTest")]
    [Export(typeof(UndoRedoViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class UndoRedoViewModel : KeyBindingLayoutItem
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
            IoC.Get<IOutput>().AppendLine("Test");
        }

        public override GestureScope GestureScope => UndoRedoScope;
        
        [Export] public static GestureScope UndoRedoScope = new GestureScope("{C9D94614-906F-4960-BA79-58DED45722F0}", "UndoRedo");
    }
}
