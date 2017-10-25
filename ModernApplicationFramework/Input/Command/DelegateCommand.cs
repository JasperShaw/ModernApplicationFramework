using System;
using System.Diagnostics;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Input.Command
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        private event EventHandler canExecuteChanged;

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            Validate.IsNotNull(execute, nameof(execute));
            _canExecute = canExecute;
            _execute = execute;
        }

        public void RaiseCanExecuteChanged()
        {
            canExecuteChanged.RaiseEvent(this);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                canExecuteChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                canExecuteChanged -= value;
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
