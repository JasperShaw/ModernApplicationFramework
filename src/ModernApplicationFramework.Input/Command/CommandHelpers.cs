using System;
using System.Windows.Input;

namespace ModernApplicationFramework.Input.Command
{
    public static class CommandHelpers
    {
        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, null);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, InputGesture inputGesture)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, Key key)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, (InputGesture)new KeyGesture(key));
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, InputGesture inputGesture, InputGesture inputGesture2)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture, inputGesture2);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, null);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, Key key)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, (InputGesture)new KeyGesture(key));
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture, InputGesture inputGesture2)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture, InputGesture inputGesture2, InputGesture inputGesture3, InputGesture inputGesture4)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2, inputGesture3, inputGesture4);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, Key key, ModifierKeys modifierKeys, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler)
        {
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, (InputGesture)new KeyGesture(key, modifierKeys));
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, string srid1, string srid2)
        {

            var kgc = new KeyGestureConverter();
            var gesture = kgc.ConvertFromInvariantString(srid1 + "," + srid2) as KeyGesture;
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, gesture);
        }

        public static void RegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, string srid1, string srid2)
        {
            var kgc = new KeyGestureConverter();
            var gesture = kgc.ConvertFromInvariantString(srid1 + "," + srid2) as KeyGesture;
            InternalRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, gesture);
        }



        private static void InternalRegisterCommandHandler(Type controlType, ICommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecute, params InputGesture[] inputGestures)
        {
            CommandManager.RegisterClassCommandBinding(controlType, new CommandBinding(command, executedRoutedEventHandler, canExecute));
            if (inputGestures == null)
                return;
            foreach (var gesture in inputGestures)
                CommandManager.RegisterClassInputBinding(controlType, new InputBinding(command, gesture));
        }
    }
}
