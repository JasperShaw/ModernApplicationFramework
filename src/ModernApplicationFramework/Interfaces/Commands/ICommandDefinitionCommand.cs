using System;
using System.Windows.Input;

namespace ModernApplicationFramework.Interfaces.Commands
{
    public interface ICommandDefinitionCommand : ICommand
    {
        event EventHandler Executed;
    }
}