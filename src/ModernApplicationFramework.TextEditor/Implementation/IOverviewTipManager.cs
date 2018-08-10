using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IOverviewTipManager
    {
        bool UpdateTip(IVerticalScrollBar margin, MouseEventArgs e, ToolTip tip);
    }
}