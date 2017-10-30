using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    ///     Special <see cref="CommandDefinition" /> for menu controller commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class CommandMenuControllerDefinition : CommandDefinitionBase
    {
        public override CommandControlTypes ControlType => CommandControlTypes.MenuToolbar;

        /// <summary>
        ///     The containing item command definitions
        /// </summary>
        public abstract ObservableCollection<CommandBarItemDefinition> Items { get; set; }

        public override bool IsList => false;
    }
}