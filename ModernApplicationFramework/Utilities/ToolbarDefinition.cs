using System.Windows.Controls;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Utilities.Service;
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

        public Dock Position { get; }

        public int SortOrder { get; }

        public bool VisibleOnLoad { get; }

        public ToolBar ToolBar { get; protected set; }
    }

    public class ToolbarDefinition<T> : ToolbarDefinition where T : ToolBar
    {
        public ToolbarDefinition(int sortOrder, bool visibleOnLoad, Dock position)
            : base(null, sortOrder, visibleOnLoad, position)
        {
            var t = IoC.Get<IToolbarService>().GetToolbar(typeof (T));
            ToolBar = t;
        }
    }
}