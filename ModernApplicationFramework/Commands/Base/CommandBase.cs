using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernApplicationFramework.Commands.Base
{
    public abstract class CommandBase : ICommand
    {
        protected readonly Func<object, bool> CanExecuteMethod;
        protected readonly Func<object, Task> ExecuteMethod;
        private List<WeakReference> _canExecuteChangedHandlers;

        protected CommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            ExecuteMethod = args =>
            {
                executeMethod(args);
                return Task.Delay(0);
            };
            CanExecuteMethod = canExecuteMethod;
        }

        protected CommandBase(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            ExecuteMethod = executeMethod;
            CanExecuteMethod = canExecuteMethod;
        }

        async void ICommand.Execute(object parameter)
        {
            await Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter) => CanExecute(parameter);

        public virtual event EventHandler CanExecuteChanged
        {
            add { WeakEventHandlerManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2); }
            remove { WeakEventHandlerManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value); }
        }

        protected virtual void OnCanExecuteChanged()
        {
            WeakEventHandlerManager.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected async Task Execute(object parameter)
        {
            await ExecuteMethod(parameter);
        }

        protected bool CanExecute(object parameter)
        {
            return CanExecuteMethod == null || CanExecuteMethod(parameter);
        }
    }
}