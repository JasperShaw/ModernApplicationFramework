using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Controls.ListBoxes;
using ModernApplicationFramework.Interfaces.Views;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Customize.Views
{
    public partial class ToolBarsPageView : IToolBarsPageView
    {
        public CheckableListBox ToolBarListBox => ListBox;
        public DropDownDialogButton ModifySelectionButton => DropDownButton;

        public ToolBarsPageView()
        {
            InitializeComponent();
            FocusHelper.FocusPossiblyUnloadedElement(ToolBarListBox);
        }
    }
}