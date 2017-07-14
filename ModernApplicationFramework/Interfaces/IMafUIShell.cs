using System;

namespace ModernApplicationFramework.Interfaces
{
    public interface IMafUIShell
    {
        int GetDialogOwnerHwnd(out IntPtr phwnd);

        int EnableModeless(int fEnable);

        int CenterDialogOnWindow(IntPtr hwndDialog, IntPtr hwndParent);
    }
}
