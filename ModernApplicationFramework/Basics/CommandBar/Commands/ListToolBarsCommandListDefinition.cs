using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(ListToolBarsCommandListDefinition))]
    public class ListToolBarsCommandListDefinition : CommandListDefinition
    {
        public override string Name => Commands_Resources.ListToolBarsCommandListDefinition_Name;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
    }
}