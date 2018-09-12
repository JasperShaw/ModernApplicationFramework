using System;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
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

        private int QueryTabKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new TabKeyCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds,
                commandText);
        }

        private int QueryBackTabKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new BackTabKeyCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryDeleteKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new DeleteKeyCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
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

        private int QueryUpKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new UpKeyCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryDownKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new DownKeyCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryDocumentEndStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new DocumentEndCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryDocumentStartStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new DocumentStartCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryLineStartStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new LineStartCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryLineStartExtendStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new LineStartExtendCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryLineEndStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new LineEndCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryLineEndExtendStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new LineEndExtendCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryPageDownKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new PageDownKeyCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryPageUpKeyStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new PageUpKeyCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QuerySelectAllStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new SelectAllCommandArgs(view, buffer), pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryCutStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new CutCommandArgs(view, buffer),
                pguidCmdGroup, commandCount, prgCmds, commandText);
        }

        private int QueryCopyStatus(Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new CopyCommandArgs(view, buffer), pguidCmdGroup, commandCount,
                prgCmds, commandText);
        }

        private int QueryPasteStatus(ref Guid pguidCmdGroup, uint commandCount, Olecmd[] prgCmds, IntPtr commandText)
        {
            return GetCommandState((view, buffer) => new PasteCommandArgs(view, buffer), pguidCmdGroup, commandCount,
                prgCmds, commandText);
        }
    }
}