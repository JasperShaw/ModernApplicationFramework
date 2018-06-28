using System;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Input.Command
{
    public abstract class CommandDefinitionCommand : AbstractCommandWrapper, ICommandDefinitionCommand
    {
        public event EventHandler Executed;

        protected CommandDefinitionCommand()
        {
            WrappedCommand = new CommandEx(OnExecuteInternal, OnCanExecute);
        }

        protected CommandDefinitionCommand(object args)
        {
            WrappedCommand = new CommandEx(o => OnExecuteInternal(args), o => OnCanExecute(args));
        }

        protected abstract bool OnCanExecute(object parameter);

        protected abstract void OnExecute(object parameter);

        private void OnExecuteInternal(object parameter)
        {
            OnExecute(parameter);
            OnExecuted();
        }

        protected virtual void OnExecuted()
        {
            Executed?.Invoke(this, EventArgs.Empty);
        }
    }
}