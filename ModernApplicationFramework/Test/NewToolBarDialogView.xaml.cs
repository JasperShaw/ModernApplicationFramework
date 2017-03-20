namespace ModernApplicationFramework.Test
{
    public partial class NewToolBarDialogView : INewToolBarView
    {
        public NewToolBarDialogView()
        {
            InitializeComponent();
            ToolBarNameTextBox.TargetUpdated += ToolBarNameTextBox_TargetUpdated;
        }

        private void ToolBarNameTextBox_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ToolBarNameTextBox.SelectionStart = 0;
            ToolBarNameTextBox.SelectionLength = ToolBarNameTextBox.Text.Length;
        }

        public void SelectTextBox()
        {
            ToolBarNameTextBox.SelectionStart = 0;
            ToolBarNameTextBox.SelectionLength = ToolBarNameTextBox.Text.Length;
            ToolBarNameTextBox.Focus();
        }
    }
}
