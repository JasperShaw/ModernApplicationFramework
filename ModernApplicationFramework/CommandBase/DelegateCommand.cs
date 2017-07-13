using System;
using System.Diagnostics;
using System.Windows.Input;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.CommandBase
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        private event EventHandler _canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                _canExecuteChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                _canExecuteChanged -= value;
            }
        }

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            Validate.IsNotNull(execute, "execute");
            _canExecute = canExecute;
            _execute = execute;
        }

        public void RaiseCanExecuteChanged()
        {
            _canExecuteChanged.RaiseEvent(this);
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
