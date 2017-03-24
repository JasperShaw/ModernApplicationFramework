using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.MVVM.Controls.Toolbars
{
    public static class ToolBarDefinitions
    {
        [Export]
        public static ToolbarDefinition Standard = new ToolbarDefinition<StandardToolbar>("Standard", 0, true, Dock.Top);
    }
}
