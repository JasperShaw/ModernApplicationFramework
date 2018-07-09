using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AddCategoryCommandDefinition))]
    public class AddCategoryCommandDefinition : CommandDefinition<IAddCategoryCommand>
    {
        public override string Name => ToolboxResources.AddCategoryCommand_Name;

        public override string NameUnlocalized =>
            ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.AddCategoryCommand_Name),
                CultureInfo.InvariantCulture);

        public override string Text => ToolboxResources.AddCategoryCommand_Text;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{D7D3206E-0BBD-41E4-96DF-07EA57571586}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }
}
