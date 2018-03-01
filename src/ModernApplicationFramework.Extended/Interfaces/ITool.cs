using System.Windows.Input;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface ITool : ILayoutItemBase
    {
        ICommand CloseCommand { get; }
        double PreferredHeight { get; }
        PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        bool IsVisible { get; set; }
    }
}