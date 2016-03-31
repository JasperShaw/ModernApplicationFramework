using System;
using System.Windows.Input;

namespace ModernApplicationFramework.Commands
{
    public abstract class CommandDefinition
    {
        protected CommandDefinition()
        {
            
        }

        protected CommandDefinition(ICommand command)
        {
            Command = command;
        }
        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract string ToolTip { get; }
        public abstract Uri IconSource { get; }
        public abstract bool CanShowInMenu { get; }
        public abstract bool CanShowInToolbar { get; }
        public virtual ICommand Command { get; }
    }
}
