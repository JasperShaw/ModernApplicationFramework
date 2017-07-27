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
            Initialized += UndoRedoTextBox_Initialized;
            Unloaded += UndoRedoTextBox_Unloaded;
        }

        private void UndoRedoTextBox_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= UndoRedoTextBox_Unloaded;
            IoC.Get<IKeyGestureService>().Remove(this);
        }

        private void UndoRedoTextBox_Initialized(object sender, EventArgs e)
        {
            Initialized -= UndoRedoTextBox_Initialized;
            IoC.Get<IKeyGestureService>().Register(this);
        }

        public CommandGestureCategory GestureCategory => UndoRedoViewModel.UndoRedoCategory;
        public UIElement BindableElement => this;
    }
}
