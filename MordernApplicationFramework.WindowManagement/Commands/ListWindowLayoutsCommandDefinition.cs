using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ListWindowLayoutsCommandDefinition))]
    public sealed class ListWindowLayoutsCommandDefinition : CommandListDefinition
    {
        public override string Name => WindowManagement_Resources.ApplyWindowLayoutListCommandDefinition_Name;

        public override string NameUnlocalized => WindowManagement_Resources.ResourceManager.GetString(
            "ApplyWindowLayoutListCommandDefinition_Name",
            CultureInfo.InvariantCulture);

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
    }
}
