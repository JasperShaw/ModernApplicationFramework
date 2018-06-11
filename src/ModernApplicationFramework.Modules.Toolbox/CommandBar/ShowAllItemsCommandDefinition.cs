using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ShowAllItemsCommandDefinition))]
    public class ShowAllItemsCommandDefinition : CommandDefinition
    {
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

        public ShowAllItemsCommandDefinition()
        {
            Command = new UICommand(ShowAll, CanShowAll);
            IsChecked = true;
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
