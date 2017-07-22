using System;

namespace ModernApplicationFramework.CommandBase
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of a <see cref="T:ModernApplicationFramework.CommandBase.Command`1" /> with a parameter of type <see langword="object" />
    /// </summary>
    /// <seealso cref="T:System.Object" />
    public class ObjectCommand : Command<object>
    {
        public ObjectCommand(Action<object> execute)
            : this(execute, o => true)
        {
        }

        public ObjectCommand(Action<object> execute, Func<object, bool> canExecute) 
            : base(execute, canExecute)
        {
        }
    }
}
