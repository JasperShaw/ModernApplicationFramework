using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ToolboxNodeDownCommandDefinition : CommandDefinition<IToolboxNodeDownCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.ToolboxNodeDownCommand_Name),
            CultureInfo.InvariantCulture);

        public override string Name => ToolboxResources.ToolboxNodeDownCommand_Name;

        public override string Text => ToolboxResources.ToolboxNodeDownCommand_Text;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{FC1C2BD3-A600-4C0D-BE5A-63DE8EED2EA9}");
    }
}
