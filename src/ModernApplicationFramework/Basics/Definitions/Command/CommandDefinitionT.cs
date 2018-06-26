using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandDefinition<T> : CommandDefinition where T : ICommandDefinitionCommand
    {
        public sealed override ICommand Command { get; }

        public override object CommandParamenter { get; set; }

        protected CommandDefinition()
        {
            Command = IoC.Get<T>();
        }
    }

    public interface ICommandDefinitionCommand : ICommand
    {
        object CommandParamenter { get; set; }
    }

    public abstract class CommandDefinitionCommand : AbstractCommandWrapper, ICommandDefinitionCommand
    {
        protected CommandDefinitionCommand()
        {
            WrappedCommand = new Input.Command.Command(Execute, CanExecute);
        }

        protected abstract bool CanExecute();

        protected abstract void Execute();

        public object CommandParamenter { get; set; }
    }
}
