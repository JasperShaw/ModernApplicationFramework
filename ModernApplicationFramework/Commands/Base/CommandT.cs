using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Commands.Base
{
    public class Command<T> : CommandBase
    {
        public Command(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(o => executeMethod((T) o), o => canExecuteMethod((T) o))
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            var genericTypeInfo = typeof (T).GetTypeInfo();

            if (!genericTypeInfo.IsValueType)
                return;
            if ((!genericTypeInfo.IsGenericType) || (!typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo())))
                throw new InvalidCastException();
        }

        public static Command<T> FromAsyncHandler(Func<T, Task> executeMethod) => new Command<T>(executeMethod);  

        public static Command<T> FromAsyncHandler(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod) => new Command<T>(executeMethod, canExecuteMethod);

        public virtual bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        public virtual async Task Execute(T parameter)
        {
            await base.Execute(parameter);
        }

        private Command(Func<T, Task> executeMethod) : this(executeMethod, o => true)
        {
            
        }

        private Command(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod) : base(o => executeMethod((T)o), o => canExecuteMethod((T)o))
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
        } 
    }
}
