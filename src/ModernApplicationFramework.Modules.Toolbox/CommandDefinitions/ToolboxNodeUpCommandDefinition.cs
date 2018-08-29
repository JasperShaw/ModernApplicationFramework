using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ToolboxNodeUpCommandDefinition : CommandDefinition<IToolboxNodeUpCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.ToolboxNodeUpCommand_Name),
            CultureInfo.InvariantCulture);

        public override string Name => ToolboxResources.ToolboxNodeUpCommand_Name;
        public override string Text => ToolboxResources.ToolboxNodeUpCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.ToolsCategory;
        public override Guid Id => new Guid("{3543E589-5D75-4CF5-88BC-254A14578C69}");
    }
}
