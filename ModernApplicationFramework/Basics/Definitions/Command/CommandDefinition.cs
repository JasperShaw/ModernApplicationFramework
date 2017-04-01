using System.Windows.Input;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandDefinition : DefinitionBase
    {
        public virtual ICommand Command { get; }

        public sealed override bool IsList => false;

        public override CommandControlTypes ControlType => CommandControlTypes.Button;

        public virtual object CommandParamenter { get; set; }

        public virtual bool IsChecked { get; set; }

        protected CommandDefinition()
        {
        }

        protected CommandDefinition(ICommand command)
        {
            Command = command;
        }
    }
}