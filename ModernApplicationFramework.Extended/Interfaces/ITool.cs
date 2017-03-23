using ModernApplicationFramework.Extended.Core.Pane;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface ITool : ILayoutItemBase
    {
        double PreferredHeight { get; }
        PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        bool IsVisible { get; set; }
    }
}