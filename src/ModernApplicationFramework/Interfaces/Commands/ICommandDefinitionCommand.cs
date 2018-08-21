using System;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Interfaces.Commands
{
    public interface ICommandDefinitionCommand : ICommand
    {
        event EventHandler Executed;

        event EventHandler CommandChanged;

        CommandStatus Status { get; }

        bool Enabled { get; set; }

        bool Checked { get; set; }

        bool Visible { get; set; }

        bool Supported { get; set; }
    }
}