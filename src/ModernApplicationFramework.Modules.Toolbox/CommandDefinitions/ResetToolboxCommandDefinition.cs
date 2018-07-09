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
    [Export(typeof(ResetToolboxCommandDefinition))]
    public class ResetToolboxCommandDefinition : CommandDefinition<IResetToolboxCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.ResetToolboxCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.ResetToolboxCommand_Name;
        public override string Text => ToolboxResources.ResetToolboxCommand_Text;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{BF0ED4C1-518C-4B30-8FD3-2085A19C63D2}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }
}
