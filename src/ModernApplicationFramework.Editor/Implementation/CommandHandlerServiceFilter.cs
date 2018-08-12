using System;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class CommandHandlerServiceFilter : ICommandTarget
    {
        private readonly SimpleTextViewWindow _textViewWindow;
        private ICommandTarget _nextCommandTarget;
        private ICommandHandlerServiceAdapter _commandHandlerServiceAdapter;

        public CommandHandlerServiceFilter(SimpleTextViewWindow textViewWindow)
        {
            var simpleTextViewWindow = textViewWindow;
            _textViewWindow = simpleTextViewWindow ?? throw new ArgumentNullException(nameof(textViewWindow));
        }

        private void EnsureCommandHandlerServiceAdapter()
        {
            if (_commandHandlerServiceAdapter != null)
                return;
            _commandHandlerServiceAdapter = new CommandHandlerServiceAdapter(_textViewWindow.TextView, _nextCommandTarget);
        }

        public void Initialize(ICommandTarget nextCommandTarget)
        {
            if (_nextCommandTarget != null)
                throw new InvalidOperationException("CommandHandlerServiceFilter cannot be initialized more than once.");
            _nextCommandTarget = nextCommandTarget;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            EnsureCommandHandlerServiceAdapter();
            return _commandHandlerServiceAdapter.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            EnsureCommandHandlerServiceAdapter();
            return _commandHandlerServiceAdapter.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }
    }
}