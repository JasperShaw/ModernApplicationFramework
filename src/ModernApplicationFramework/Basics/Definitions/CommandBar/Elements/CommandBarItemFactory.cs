using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    [Export]
    internal class CommandBarItemFactory
    {
        [ImportMany] internal List<Lazy<CommandBarItem>> RegisteredCommandBarItems;
    }
}