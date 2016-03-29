using ModernApplicationFramework.MVVM.Core;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface ITool : ILayoutItem
    {
        PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        double PreferredHeight { get; }
        bool IsVisible { get; set; }
    }
}
