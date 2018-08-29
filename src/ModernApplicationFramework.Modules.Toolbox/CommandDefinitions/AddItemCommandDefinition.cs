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
    internal class AddItemCommandDefinition : CommandDefinition<IAddItemCommand>
    {
        public override string Name => ToolboxResources.AddItemCommand_Name;

        public override string NameUnlocalized =>
            ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.AddItemCommand_Name),
                CultureInfo.InvariantCulture);

        public override string Text => ToolboxResources.AddItemCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.ToolsCategory;
        public override Guid Id => new Guid("{E89CE9AE-B927-49AE-95C9-B55726CDB475}");
    }
}
