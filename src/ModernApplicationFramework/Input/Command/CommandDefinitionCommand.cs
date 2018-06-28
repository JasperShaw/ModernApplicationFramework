using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Input.Command
{
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