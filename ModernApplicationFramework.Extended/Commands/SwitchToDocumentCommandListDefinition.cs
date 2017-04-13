using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    public class SwitchToDocumentCommandListDefinition : CommandListDefinition
    {
        public override string Name => "Window List";

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
    }
}