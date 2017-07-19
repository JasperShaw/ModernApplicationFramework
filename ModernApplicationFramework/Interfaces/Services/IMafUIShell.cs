using System;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMafUIShell
    {
        int GetDialogOwnerHwnd(out IntPtr phwnd);

        int EnableModeless(int fEnable);

        int CenterDialogOnWindow(IntPtr hwndDialog, IntPtr hwndParent);

        int GetAppName(out string pbstrAppName);

    }
}
