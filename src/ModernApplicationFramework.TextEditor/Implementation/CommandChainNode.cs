using System;

namespace ModernApplicationFramework.TextEditor.Implementation
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

        public int QueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            return Next.InnerQueryStatus(ref commandGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            return Next.InnerExec(ref commandGroup, commandId, nCmdexecopt, input, output);
        }

        public int InnerExec(ref Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            if (!CallFilterObject)
                return Next.InnerExec(ref commandGroup, commandId, nCmdexecopt, input, output);
            return FilterObject.Exec(ref commandGroup, commandId, nCmdexecopt, input, output);
        }

        public int InnerQueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            if (!CallFilterObject)
                return Next.InnerQueryStatus(ref commandGroup, cCmds, prgCmds, pCmdText);
            return FilterObject.QueryStatus(ref commandGroup, cCmds, prgCmds, pCmdText);
        }

        public void Dispose()
        {
            FilterObject = null;
            Next = null;
            ContainingTextView = null;
        }
    }
}