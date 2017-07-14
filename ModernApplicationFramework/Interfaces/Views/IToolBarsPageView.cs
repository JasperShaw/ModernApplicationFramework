using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Controls.ListBoxes;

namespace ModernApplicationFramework.Interfaces.Views
{
    internal interface IToolBarsPageView
    {
        CheckableListBox ToolBarListBox { get; }

        DropDownDialogButton ModifySelectionButton { get; }
    }
}
