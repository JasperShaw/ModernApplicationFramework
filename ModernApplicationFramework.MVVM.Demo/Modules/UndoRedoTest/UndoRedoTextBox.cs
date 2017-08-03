using System;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.TextBoxes;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest
{
    public class UndoRedoTextBox : TextBox, ICanHaveInputBindings
    {
        public UndoRedoTextBox()
        {  
            Loaded += UndoRedoTextBox_Initialized;
            Unloaded += UndoRedoTextBox_Unloaded;
        }

        private void UndoRedoTextBox_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= UndoRedoTextBox_Unloaded;
            IoC.Get<IKeyGestureService>().RemoveModel(this);
            Loaded += UndoRedoTextBox_Initialized;
        }

        private void UndoRedoTextBox_Initialized(object sender, EventArgs e)
        {
            Loaded -= UndoRedoTextBox_Initialized;
            IoC.Get<IKeyGestureService>().AddModel(this);
            Unloaded += UndoRedoTextBox_Unloaded;
        }

        public GestureScope GestureScope => UndoRedoViewModel.UndoRedoScope;
        public UIElement BindableElement => this;
    }
}
