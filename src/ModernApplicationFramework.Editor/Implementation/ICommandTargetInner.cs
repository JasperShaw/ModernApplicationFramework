using System;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal interface ICommandTargetInner
    {
        int InnerExec(Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output);

        int InnerQueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText);
    }
}