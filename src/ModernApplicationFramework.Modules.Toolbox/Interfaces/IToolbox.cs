using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolbox : ITool
    {
        IDropTarget ToolboxDropHandler { get; }
    }
}
