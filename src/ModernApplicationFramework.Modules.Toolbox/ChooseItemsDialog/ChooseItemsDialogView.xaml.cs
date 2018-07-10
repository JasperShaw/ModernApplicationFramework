using System;
using System.Windows;
using System.Windows.Interop;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public partial class ChooseItemsDialogView
    {
        public ChooseItemsDialogView()
        {
            InitializeComponent();
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

        internal void EnsureDialogVisible()
        {
            BringIntoView();
            Focus();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this))?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        internal enum ButtonType
        {
            Ok,
            Cancel,
            Reset
        }
    }
}
