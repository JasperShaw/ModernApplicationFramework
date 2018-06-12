using System.Collections.Generic;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolbox : ITool
    {
        IDropTarget ToolboxDropHandler { get; }

        IDragSource ToolboxDragHandler { get; }

        bool ShowAllItems { get; set; }

        IToolboxNode SelectedNode { get; set; }

        IToolboxCategory SelectedCategory { get; }

        IReadOnlyCollection<IToolboxCategory> CurrentLayout { get; }

        void RefreshView();
    }
}
