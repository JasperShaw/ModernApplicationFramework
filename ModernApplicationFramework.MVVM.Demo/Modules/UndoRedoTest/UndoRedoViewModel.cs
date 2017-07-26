using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest
{
    [DisplayName("UndoRedoTest")]
    [Export(typeof(UndoRedoViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class UndoRedoViewModel : Extended.Core.LayoutItems.LayoutItem, ICanHaveInputBindings
    {
        public override bool ShouldReopenOnStart => true;

        private string _test;

        public UndoRedoViewModel()
        {
            DisplayName = "UndoRedoTest";
            GestureCategory = new CommandGestureCategory("RedoUndo");
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


        protected override void OnViewLoaded(object view)
        {
            if (view is UIElement element)
            {
                BindableElement = element;
                BindGestures();
            }
        }

        private void SetValue()
        {
            Text += "5";
        }

        public CommandGestureCategory GestureCategory { get; }
        
        public UIElement BindableElement { get; private set; }
        
        public void BindGestures()
        {
            IoC.Get<IKeyGestureService>().BindKeyGestures(this);
        }
    }
}
