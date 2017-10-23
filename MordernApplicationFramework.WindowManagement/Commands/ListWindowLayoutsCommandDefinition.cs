using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ListWindowLayoutsCommandDefinition))]
    public sealed class ListWindowLayoutsCommandDefinition : CommandListDefinition
    {
        public override string Name => WindowManagement_Resources.ApplyWindowLayoutListCommandDefinition;
        public override string NameUnlocalized => "Apply Window Layout List";
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
    }
}
