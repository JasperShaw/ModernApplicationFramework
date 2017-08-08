using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
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

            Deactivated += UndoRedoViewModel_Deactivated;
        }

        private void UndoRedoViewModel_Deactivated(object sender, DeactivationEventArgs e)
        {
            if (!e.WasClosed)
                return;
            IoC.Get<IKeyGestureService>().RemoveModel(this);
            BindableElement = null;
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
            if (view is Control element)
            {
                BindableElement = element;
                IoC.Get<IKeyGestureService>().AddModel(this);
            }
        }


        private void SetValue()
        {
            Text += "5";
        }

        public GestureScope GestureScope => UndoRedoScope;
        
        public UIElement BindableElement { get; private set; }
               
        [Export] public static GestureScope UndoRedoScope = new GestureScope("{C9D94614-906F-4960-BA79-58DED45722F0}", "UndoRedo");
    }
}
