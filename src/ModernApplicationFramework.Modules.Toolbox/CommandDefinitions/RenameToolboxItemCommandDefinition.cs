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
    internal class RenameToolboxItemCommandDefinition : CommandDefinition<IRenameToolboxItemCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.RenameItemCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.RenameItemCommand_Name;
        public override string Text => ToolboxResources.RenameItemCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.ToolsCategory;
        public override Guid Id => new Guid("{03E545BE-2ED2-4396-B641-A9582FFF3452}");
    }
}
