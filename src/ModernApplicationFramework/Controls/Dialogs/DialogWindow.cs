using System;
using System.Windows;
using ModernApplicationFramework.Controls.Primitives;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Controls.Dialogs
{
    /// <summary>
    /// A special window used for dialogs
    /// </summary>
    /// <seealso cref="WindowBase" />
    public class DialogWindow : WindowBase
    {
        public DialogWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;
            HasDialogFrame = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            NativeMethods.RemoveIcon(this);
        }

        public bool? ShowModal()
        {
            int num = WindowHelper.ShowModal(this);
            if (num == 0)
                return new bool?();
            return num == 1;
        }
    }
}