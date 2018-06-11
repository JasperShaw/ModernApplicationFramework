using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ShowAllItemsCommandDefinition))]
    public class ShowAllItemsCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;

        public override string NameUnlocalized => "Show All";
        public override string Text => "Show All";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{BB1C5EAB-A114-4A06-995C-E311F9DA8C11}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ShowAllItemsCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox;
            Command = new UICommand(ShowAll, CanShowAll);
        }

        private void ShowAll()
        {
            
        }

        private bool CanShowAll()
        {
            return true;
        }
    }
}
