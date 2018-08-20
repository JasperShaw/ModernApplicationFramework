using System;

namespace ModernApplicationFramework.Editor.Interop
{

    //TODO: move to other project
    public interface ICommandTarget
    {
        int QueryStatus(Guid commandGroup, uint commandCount, Olecmd[] commands, IntPtr pCmdText);

        int Exec(Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output);
    }
}