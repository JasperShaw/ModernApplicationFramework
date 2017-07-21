using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    ///     Special <see cref="CommandDefinition" /> for menu controllers models
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase" />
    public abstract class CommandMenuControllerDefinition : DefinitionBase
    {
        public override CommandControlTypes ControlType => CommandControlTypes.MenuToolbar;

        /// <summary>
        ///     The containing item command definitions
        /// </summary>
        public abstract ObservableCollection<CommandBarItemDefinition> Items { get; set; }

        public override bool IsList => false;
    }
}