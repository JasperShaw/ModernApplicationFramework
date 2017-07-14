using System.Windows;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Controls.ListBoxes;
using ModernApplicationFramework.Interfaces.Views;

namespace ModernApplicationFramework.Basics.CustomizeDialog.Views
{
    public partial class ToolBarsPageView : IToolBarsPageView
    {
        public CheckableListBox ToolBarListBox => ListBox;
        public DropDownDialogButton ModifySelectionButton => DropDownButton;

        public ToolBarsPageView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ToolBarListBox.Focus();
            Loaded -= OnLoaded;
        }
    }
}