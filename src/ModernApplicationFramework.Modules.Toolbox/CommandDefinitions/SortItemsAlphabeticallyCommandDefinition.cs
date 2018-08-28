using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class SortItemsAlphabeticallyCommandDefinition : CommandDefinition<ISortItemsAlphabeticallyCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.SortItemsAlphabeticallyCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.SortItemsAlphabeticallyCommand_Name;
        public override string Text => ToolboxResources.SortItemsAlphabeticallyCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandCategories.ToolsCategory;
        public override Guid Id => new Guid("{A2C9C04A-75EB-44A5-9272-D6B9DEA1D417}");
    }
}
