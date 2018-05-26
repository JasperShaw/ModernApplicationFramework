using GongSolutions.Wpf.DragDrop;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public interface IToolbox : ITool
    {
        IDropTarget ToolboxDropHandler { get; }
    }
}
