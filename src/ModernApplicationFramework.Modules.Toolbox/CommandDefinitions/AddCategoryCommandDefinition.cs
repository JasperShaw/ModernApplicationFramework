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
    internal class AddCategoryCommandDefinition : CommandDefinition<IAddCategoryCommand>
    {
        public override string Name => ToolboxResources.AddCategoryCommand_Name;

        public override string NameUnlocalized =>
            ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.AddCategoryCommand_Name),
                CultureInfo.InvariantCulture);

        public override string Text => ToolboxResources.AddCategoryCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandCategories.ToolsCategory;
        public override Guid Id => new Guid("{D7D3206E-0BBD-41E4-96DF-07EA57571586}");
    }
}
