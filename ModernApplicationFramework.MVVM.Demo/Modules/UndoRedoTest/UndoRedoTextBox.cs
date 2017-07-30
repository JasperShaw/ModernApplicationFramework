using System;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls.TextBoxes;
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
            IoC.Get<IKeyGestureService>().Remove(this);
            Loaded += UndoRedoTextBox_Initialized;
        }

        private void UndoRedoTextBox_Initialized(object sender, EventArgs e)
        {
            Loaded -= UndoRedoTextBox_Initialized;
            IoC.Get<IKeyGestureService>().Add(this);
            Unloaded += UndoRedoTextBox_Unloaded;
        }

        public CommandGestureCategory GestureCategory => UndoRedoViewModel.UndoRedoCategory;
        public UIElement BindableElement => this;
    }
}
