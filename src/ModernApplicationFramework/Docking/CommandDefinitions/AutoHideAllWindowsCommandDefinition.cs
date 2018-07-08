using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(AutoHideAllWindowsCommandDefinition))]
    public sealed class AutoHideAllWindowsCommandDefinition : CommandDefinition<IAutoHideAllWindowsCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;
        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("AutoHideAllWindowsCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.AutoHideAllWindowsCommandDefinition_Text;
        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override Guid Id => new Guid("{ABF14EC8-CC6D-4B2A-A1A1-9A0F44690266}");
    }
}