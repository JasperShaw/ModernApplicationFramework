using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal interface ICommandTargetInner
    {
        int InnerExec(ref Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output);

        int InnerQueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText);
    }
}