using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    public class WindowSelectCommandDefinition : CommandDefinition<IWindowSelectCommand>
    {
        public override string NameUnlocalized => "Window";
        public override string Text => CommandsResources.WindowSelectCommandText;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{40A3EA90-739D-4569-AF50-3C69C7D44438}");
    }
}
