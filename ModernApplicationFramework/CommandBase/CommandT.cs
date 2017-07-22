using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernApplicationFramework.CommandBase
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract <see cref="ICommand" /> whose delegates can be attached for <see cref="Execute" /> and <see cref="CanExecute" />.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    public abstract class Command<T> : Base.CommandBase
    {
        protected Command(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(o => executeMethod((T) o), o => canExecuteMethod((T) o))
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            var genericTypeInfo = typeof(T).GetTypeInfo();

            if (!genericTypeInfo.IsValueType)
                return;
            if (!genericTypeInfo.IsGenericType
                || !typeof(Nullable<>).GetTypeInfo()
                                      .IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo()))
                throw new InvalidCastException();
        }


        ///<summary>
        ///Determines if the command can execute by invoked the <see cref="Func{T,Bool}"/> provided during construction.
        ///</summary>
        ///<param name="parameter">Data used by the command to determine if it can execute.</param>
        ///<returns>
        ///<see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        ///</returns>
        public virtual bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        ///<summary>
        ///Executes the command and invokes the <see cref="Action{T}"/> provided during construction.
        ///</summary>
        ///<param name="parameter">Data used by the command.</param>
        public virtual async Task Execute(T parameter)
        {
            await base.Execute(parameter);
        }
    }
}