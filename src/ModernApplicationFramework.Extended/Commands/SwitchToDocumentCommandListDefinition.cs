using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SwitchToDocumentCommandListDefinition))]
    public class SwitchToDocumentCommandListDefinition : CommandListDefinition
    {
        public override string Name => Commands_Resources.SwitchToDocumentCommandListDefinition_Name;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("SwitchToDocumentCommandListDefinition_Name",
                CultureInfo.InvariantCulture);

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
    }
}