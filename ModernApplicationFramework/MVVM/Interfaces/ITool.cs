using ModernApplicationFramework.MVVM.Core;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ITool : ILayoutItem
    {
        double PreferredHeight { get; }
        PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        bool IsVisible { get; set; }
    }
}