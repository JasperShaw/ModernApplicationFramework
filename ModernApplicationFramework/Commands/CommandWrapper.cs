using System;
using System.Windows.Input;

namespace ModernApplicationFramework.Commands
{
    /*This class is doing some magic since the CanExecuteChanged in the base Command was not fired correctly*/

    public class CommandWrapper : ICommand
    {
        public CommandWrapper(Action executeAction, Func<bool> cantExectueFunc)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));
            if (cantExectueFunc == null)
                throw new ArgumentNullException(nameof(cantExectueFunc));
            WrappedCommand = new Command(executeAction, cantExectueFunc);
        }

        public CommandWrapper(ICommand wrappedCommand)
        {
            if (wrappedCommand == null)
                throw new ArgumentNullException(nameof(wrappedCommand));

            WrappedCommand = wrappedCommand;
        }

        public ICommand WrappedCommand { get; }

        public bool CanExecute(object parameter)
        {
            return WrappedCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            WrappedCommand.Execute(parameter);
        }
    }
}