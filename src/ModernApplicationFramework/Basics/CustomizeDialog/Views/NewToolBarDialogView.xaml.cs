using System.Windows.Data;
using ModernApplicationFramework.Interfaces.Views;

namespace ModernApplicationFramework.Basics.CustomizeDialog.Views
{
    public partial class NewToolBarDialogView : INewToolBarView
    {
        public NewToolBarDialogView()
        {
            InitializeComponent();
            ToolBarNameTextBox.TargetUpdated += ToolBarNameTextBox_TargetUpdated;
        }

        public void SelectTextBox()
        {
            ToolBarNameTextBox.SelectionStart = 0;
            ToolBarNameTextBox.SelectionLength = ToolBarNameTextBox.Text.Length;
            ToolBarNameTextBox.Focus();
        }

        private void ToolBarNameTextBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ToolBarNameTextBox.SelectionStart = 0;
            ToolBarNameTextBox.SelectionLength = ToolBarNameTextBox.Text.Length;
        }
    }
}