using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.MVVM.Controls.Toolbars
{
    public static class ToolBarDefinitions
    {
        [Export]
        public static ToolbarDefinitionOld Standard = new ToolbarDefinitionOld<StandardToolbar>("Standard", 0, true, Dock.Top);
    }
}
