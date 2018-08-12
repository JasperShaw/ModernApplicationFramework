using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.OverviewMargin
{
    public interface IOverviewTipManager
    {
        bool UpdateTip(IVerticalScrollBar margin, MouseEventArgs e, ToolTip tip);
    }
}