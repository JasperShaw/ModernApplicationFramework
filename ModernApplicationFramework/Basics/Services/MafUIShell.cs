using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(IMafUIShell))]
    public class MafUIShell : IMafUIShell
    {
        public int GetDialogOwnerHwnd(out IntPtr phwnd)
        {
            throw new NotImplementedException();
        }
    }
}
