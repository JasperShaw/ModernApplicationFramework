using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    internal class DeleteActiveToolbarCategoryCommandDefinition : CommandDefinition<IDeleteActiveToolbarCategoryCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.DeleteCategoryCommand_Name),
            CultureInfo.InvariantCulture);

        public override string Name => ToolboxResources.DeleteCategoryCommand_Name;
        public override string Text => ToolboxResources.DeleteCategoryCommand_Text;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{2A33CF7A-4C10-4FA7-A766-A45F1661F4DF}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }
}
