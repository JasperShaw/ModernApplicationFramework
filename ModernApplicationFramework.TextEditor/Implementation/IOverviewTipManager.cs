using System.Windows.Controls;
using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IOverviewTipManager
    {
        bool UpdateTip(IVerticalScrollBar margin, MouseEventArgs e, ToolTip tip);
    }
}