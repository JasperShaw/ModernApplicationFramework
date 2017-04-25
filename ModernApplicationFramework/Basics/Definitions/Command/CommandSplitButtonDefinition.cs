using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandSplitButtonDefinition : CommandDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.SplitDropDown;

        public abstract ObservableCollection<object> Items { get; set; }
    }
}
