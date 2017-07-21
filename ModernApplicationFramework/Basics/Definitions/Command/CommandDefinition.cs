using System.Windows.Input;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    /// Basic definition model used for application commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase" />
    public abstract class CommandDefinition : DefinitionBase
    {
        /// <summary>
        /// The executable command of the definition
        /// </summary>
        public virtual ICommand Command { get; }

        public sealed override bool IsList => false;

        public override CommandControlTypes ControlType => CommandControlTypes.Button;

        /// <summary>
        /// A command specific parameter 
        /// </summary>
        public virtual object CommandParamenter { get; set; }

        /// <summary>
        /// Indicated whether the command is checked or not.
        /// </summary>
        public virtual bool IsChecked { get; set; }

        /// <summary>
        /// Option to suppress the <see cref="Command"/> from execution. Default is <see langword="true"/>.
        /// Gets automatically disabled when the command was added to the exclusion list of the <see cref="ICommandBarDefinitionHost"/>
        /// </summary>
        public bool AllowExecution { get; set; } = true;

        protected CommandDefinition()
        {
        }

        protected CommandDefinition(ICommand command)
        {
            Command = command;
        }
    }
}