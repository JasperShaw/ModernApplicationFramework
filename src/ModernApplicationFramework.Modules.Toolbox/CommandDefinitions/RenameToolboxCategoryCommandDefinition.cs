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
    [Export(typeof(RenameToolboxCategoryCommandDefinition))]
    public class RenameToolboxCategoryCommandDefinition : CommandDefinition<IRenameToolboxCategoryCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.RenameCategoryCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.RenameCategoryCommand_Name;
        public override string Text => ToolboxResources.RenameCategoryCommand_Text;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{0524D1D9-DF40-4D62-85DB-966AED7F8C35}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }
}
