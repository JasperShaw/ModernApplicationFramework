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
    internal class ToggleShowAllItemsCommandDefinition : CommandDefinition<IToggleShowAllItemsCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.SortItemsAlphabeticallyCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.SortItemsAlphabeticallyCommand_Name;
        public override string Text => ToolboxResources.ShowAllNodesCommand_Text;
        public override string ToolTip => Text;
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
