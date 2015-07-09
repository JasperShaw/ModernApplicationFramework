using System;
using System.Threading.Tasks;
using ModernApplicationFramework.Commands.Base;

namespace ModernApplicationFramework.Commands
{
    public class Command : CommandBase
    {
        public Command(Action executeMethod) : this (executeMethod, () => true)
        {
        }

        public Command(Action executeMethod, Func<bool> canExecuteMethod) : base(o => executeMethod(), o => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
        }

        public static Command FromAsyncHandler(Func<Task> executeMethod) => new Command(executeMethod);

        public static Command FromAsyncHandler(Func<Task> executeMethod, Func<bool> canExecuteMethod) => new Command(executeMethod, canExecuteMethod);

        public virtual async Task Execute()
        {
            await Execute(null);
        }

        public virtual bool CanExecute()
        {
            return CanExecute(null);
        }

        private Command(Func<Task> executeMethod) : this(executeMethod, () => true)
        {
            
        }

        private Command(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
        }
    }
}
