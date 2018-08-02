﻿using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{

    //TODO: move to other project
    public interface ICommandTarget
    {
        int QueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText);

        int Exec(ref Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output);
    }
}