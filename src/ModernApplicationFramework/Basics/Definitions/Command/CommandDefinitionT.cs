using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandDefinition<T> : CommandDefinition where T : ICommandDefinitionCommand
    {
        protected CommandDefinition()
        {
            Command = IoC.Get<T>();
        }
    }

    public interface ICommandDefinitionCommand : ICommand
    {
    }

    public abstract class CommandDefinitionCommand : AbstractCommandWrapper, ICommandDefinitionCommand
    {
        protected CommandDefinitionCommand()
        {
            WrappedCommand = new CommandEx(OnExecute, OnCanExecute);
        }

        protected CommandDefinitionCommand(object args)
        {
            WrappedCommand = new CommandEx(o => OnExecute(args), o => OnCanExecute(args));
        }

        protected abstract bool OnCanExecute(object parameter);

        protected abstract void OnExecute(object parameter);
    }
}
