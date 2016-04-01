using System.Windows.Controls;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFramework.Utilities
{
    public class ToolbarDefinition
    {

        public ToolbarDefinition(ToolBar toolBar, int sortOrder, bool visibleOnLoad, Dock position)
        {
            ToolBar = toolBar;
            SortOrder = sortOrder;
            VisibleOnLoad = visibleOnLoad;
            Position = position;
        }

        public ToolBar ToolBar { get;}

        public bool VisibleOnLoad { get; }

        public Dock Position { get; }

        public int SortOrder { get; }
    }
}
