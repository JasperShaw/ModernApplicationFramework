using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandMenuControllerDefinition : DefinitionBase
    {
        public override CommandControlTypes ControlType => CommandControlTypes.MenuToolbar;

        public abstract ObservableCollection<CommandBarItemDefinition> Items { get; set; }

        public override bool IsList => false;
    }
}
