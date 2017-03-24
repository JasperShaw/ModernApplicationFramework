using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    public static class ToolBarDefinitions
    {
        [Export]
        public static ToolbarDefinition Demo = new ToolbarDefinition<DemoToolbar>("Demos", 0, true, Dock.Left);
    }
}
