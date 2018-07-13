namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog.Internal
{
    public partial class ChooseItemsDialogView
    {
        public ChooseItemsDialogView()
        {
            InitializeComponent();
        }

        internal enum ButtonType
        {
            Ok,
            Cancel,
            Reset
        }

        internal void EnsureDialogVisible()
        {
            BringIntoView();
            Focus();
        }

        internal void FocusButton(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Ok:
                    OkButton.Focus();
                    break;
                case ButtonType.Cancel:
                    CancelButton.Focus();
                    break;
                case ButtonType.Reset:
                    ResetButton.Focus();
                    break;
            }
        }
    }
}