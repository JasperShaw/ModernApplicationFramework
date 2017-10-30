using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ModernApplicationFramework.Input.Base
{
    /*This class is doing some magic since the CanExecuteChanged in the base Command was not fired correctly*/

    /// <inheritdoc />
    /// <summary>
    /// Abstract command wrapper that triggers the <see cref="ICommand.CanExecuteChanged" /> correctly
    /// </summary>
    /// <seealso cref="ICommand" />
    public abstract class AbstractCommandWrapper : ICommand
    {
        protected AbstractCommandWrapper(Action executeAction, Func<bool> cantExectueFunc)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));
            if (cantExectueFunc == null)
                throw new ArgumentNullException(nameof(cantExectueFunc));
            WrappedCommand = new Command.Command(executeAction, cantExectueFunc);
        }

        protected AbstractCommandWrapper(ICommand wrappedCommand)
        {
            WrappedCommand = wrappedCommand ?? throw new ArgumentNullException(nameof(wrappedCommand));
        }

        public ICommand WrappedCommand { get; }

        public bool CanExecute(object parameter)
        {
            return WrappedCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [DebuggerHidden]
        public void Execute(object parameter)
        {
            WrappedCommand.Execute(parameter);
        }
    }
}