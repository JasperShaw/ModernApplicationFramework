using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(SwitchToDocumentCommandListDefinition))]
    public class SwitchToDocumentCommandListDefinition : CommandListDefinition
    {
        public override string Name => Commands_Resources.SwitchToDocumentCommandListDefinition_Name;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
    }
}