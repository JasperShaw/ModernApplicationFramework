using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class RenameToolboxCategoryCommandDefinition : CommandDefinition<IRenameToolboxCategoryCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.RenameCategoryCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.RenameCategoryCommand_Name;
        public override string Text => ToolboxResources.RenameCategoryCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandCategories.ToolsCategory;
        public override Guid Id => new Guid("{0524D1D9-DF40-4D62-85DB-966AED7F8C35}");
    }
}
