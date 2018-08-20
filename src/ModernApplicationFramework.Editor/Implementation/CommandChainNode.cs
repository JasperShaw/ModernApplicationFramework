using System;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal sealed class CommandChainNode : ICommandTarget, ICommandTargetInner, IDisposable
    {
        public ICommandTarget FilterObject { get; set; }

        public ICommandTargetInner Next { get; set; }

        public SimpleTextViewWindow ContainingTextView;

        private bool CallFilterObject
        {
            get
            {
                if (FilterObject == null)
                    return false;
                if (ContainingTextView != null)
                    return ContainingTextView.CanCaretAndSelectionMapToDataBuffer;
                return true;
            }
        }

        public int QueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            return Next.InnerQueryStatus(commandGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            return Next.InnerExec(commandGroup, commandId, nCmdexecopt, input, output);
        }

        public int InnerExec(Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            if (!CallFilterObject)
                return Next.InnerExec(commandGroup, commandId, nCmdexecopt, input, output);
            return FilterObject.Exec(commandGroup, commandId, nCmdexecopt, input, output);
        }

        public int InnerQueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            if (!CallFilterObject)
                return Next.InnerQueryStatus(commandGroup, cCmds, prgCmds, pCmdText);
            return FilterObject.QueryStatus(commandGroup, cCmds, prgCmds, pCmdText);
        }

        public void Dispose()
        {
            FilterObject = null;
            Next = null;
            ContainingTextView = null;
        }
    }
}