using System.Windows;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Basics.CustomizeDialog.Views
{
    public partial class ToolBarsPageView : IToolBarsPageView
    {
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

        public CheckableListBox ToolBarListBox => ListBox;
        public DropDownDialogButton ModifySelectionButton => DropDownButton;
    }
}
