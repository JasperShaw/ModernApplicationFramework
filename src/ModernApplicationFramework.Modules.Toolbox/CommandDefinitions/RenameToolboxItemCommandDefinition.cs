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
    internal class RenameToolboxItemCommandDefinition : CommandDefinition<IRenameToolboxItemCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.RenameItemCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.RenameItemCommand_Name;
        public override string Text => ToolboxResources.RenameItemCommand_Text;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{03E545BE-2ED2-4396-B641-A9582FFF3452}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }
}
