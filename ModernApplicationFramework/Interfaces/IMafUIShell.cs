using System;

namespace ModernApplicationFramework.Interfaces
{
    public interface IMafUIShell
    {
        int GetDialogOwnerHwnd(out IntPtr phwnd);
    }
}
