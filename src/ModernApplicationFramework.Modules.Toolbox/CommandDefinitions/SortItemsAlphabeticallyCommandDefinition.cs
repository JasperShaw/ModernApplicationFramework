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
    [Export(typeof(SortItemsAlphabeticallyCommandDefinition))]
    public class SortItemsAlphabeticallyCommandDefinition : CommandDefinition<ISortItemsAlphabeticallyCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.SortItemsAlphabeticallyCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.SortItemsAlphabeticallyCommand_Name;
        public override string Text => ToolboxResources.SortItemsAlphabeticallyCommand_Text;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{A2C9C04A-75EB-44A5-9272-D6B9DEA1D417}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }
}
