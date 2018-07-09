using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    public class AddItemCommandDefinition : CommandDefinition<IAddItemCommand>
    {
        public override string NameUnlocalized => "Add";
        public override string Text => "Add";
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{E89CE9AE-B927-49AE-95C9-B55726CDB475}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }
}
