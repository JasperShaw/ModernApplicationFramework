using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(ListToolBarsCommandListDefinition))]
    public class ListToolBarsCommandListDefinition : CommandListDefinition
    {
        public override string Name => CommandBarResources.ListToolBarsCommandListDefinition_Name;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
    }
}