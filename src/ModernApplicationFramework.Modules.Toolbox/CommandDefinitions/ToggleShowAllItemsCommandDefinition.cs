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
    [Export(typeof(ToggleShowAllItemsCommandDefinition))]
    public class ToggleShowAllItemsCommandDefinition : CommandDefinition<IToggleShowAllItemsCommand>
    {
        public override string NameUnlocalized => "Show All";
        public override string Text => "Show All";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{BB1C5EAB-A114-4A06-995C-E311F9DA8C11}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public ToggleShowAllItemsCommandDefinition()
        {
            Command.Executed += Command_Executed;
        }

        private void Command_Executed(object sender, EventArgs e)
        {
            IsChecked = !IsChecked;
        }
    }
}
