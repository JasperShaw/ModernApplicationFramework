using System;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal partial class CommandHandlerServiceAdapter
    {
        

        private int QueryBackspaceKeyStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new BackspaceKeyCommandArgs(view, buffer), pguidCmdGroup,
                commandCount, prgCmds, commandText);
        }

        private int QueryReturnKeyStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new ReturnKeyCommandArgs(view, buffer), pguidCmdGroup,
                commandCount, prgCmds, commandText);
        }

        private int QueryLeftKeyStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new LeftKeyCommandArgs(view, buffer), pguidCmdGroup,
                commandCount, prgCmds, commandText);
        }

        private int QueryRightKeyStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new RightKeyCommandArgs(view, buffer), pguidCmdGroup,
                commandCount, prgCmds, commandText);
        }

        private int QueryCopyStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new CopyCommandArgs(view, buffer), pguidCmdGroup, commandCount,
                prgCmds, commandText);
        }
    }
}