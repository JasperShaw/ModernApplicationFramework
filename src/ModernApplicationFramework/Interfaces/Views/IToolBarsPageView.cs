using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Controls.Buttons.DialogButtons;
using ModernApplicationFramework.Controls.ListBoxes;

namespace ModernApplicationFramework.Interfaces.Views
{
    /// <summary>
    /// This interface specifies the structure of the UI for managing toolbars
    /// </summary>
    internal interface IToolBarsPageView
    {
        /// <summary>
        /// A list box that contains the available toolbars
        /// </summary>
        CheckableListBox ToolBarListBox { get; }

        /// <summary>
        /// A drop down button
        /// </summary>
        DropDownDialogButton ModifySelectionButton { get; }
    }
}
