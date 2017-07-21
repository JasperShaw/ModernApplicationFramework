using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Definition of the command to show all tool bars as a menu item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandListDefinition" />
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ListToolBarsCommandListDefinition))]
    public class ListToolBarsCommandListDefinition : CommandListDefinition
    {
        public override string Name => CommandBarResources.ListToolBarsCommandListDefinition_Name;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
    }
}