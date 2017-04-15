using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Interfaces.Views
{
    internal interface IToolBarsPageView
    {
        CheckableListBox ToolBarListBox { get; }

        DropDownDialogButton ModifySelectionButton { get; }
    }
}
