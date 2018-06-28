using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ToggleShowAllItemsCommandDefinition))]
    public class ToggleShowAllItemsCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;

        public override string NameUnlocalized => "Show All";
        public override string Text => "Show All";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{BB1C5EAB-A114-4A06-995C-E311F9DA8C11}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ToggleShowAllItemsCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox;
            Command = new UICommand(ShowAll, CanShowAll);
        }

        private void ShowAll()
        {
            _toolbox.ShowAllItems = !_toolbox.ShowAllItems;
            IsChecked = _toolbox.ShowAllItems;
        }

        private bool CanShowAll()
        {
            return true;
        }
    }
}
