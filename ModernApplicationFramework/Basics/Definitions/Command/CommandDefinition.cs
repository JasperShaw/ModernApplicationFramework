using System.Windows.Input;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandDefinition : DefinitionBase
    {
        protected CommandDefinition() {}

        protected CommandDefinition(ICommand command)
        {
            Command = command;
        }

        public abstract bool CanShowInMenu { get; }
        public abstract bool CanShowInToolbar { get; }
        public virtual ICommand Command { get;}
        public virtual object CommandParamenter { get; set; }
        public sealed override bool IsList => false;
        public virtual bool IsChecked { get; set; }
    }
}