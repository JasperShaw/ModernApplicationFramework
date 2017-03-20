using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    public static class ToolBarDefinitions
    {
        [Export]
        public static ToolbarDefinition Demo = new ToolbarDefinition<DemoToolbar>("Demos", 0, true, Dock.Left);
    }
}
