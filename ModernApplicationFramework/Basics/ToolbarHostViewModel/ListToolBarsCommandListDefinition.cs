using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.ToolbarHostViewModel
{
    [Export(typeof(DefinitionBase))]
    public class ListToolBarsCommandListDefinition : CommandListDefinition
    {
        public override string Name => "Toolbar List";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
    }
}
