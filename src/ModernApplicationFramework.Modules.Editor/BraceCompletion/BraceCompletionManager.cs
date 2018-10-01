using System;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal class BraceCompletionManager : IBraceCompletionManager
    {
        private ITextView _textView;
        private IBraceCompletionStack _stack;
        private IBraceCompletionAggregatorFactory _sessionFactory;
        private IGuardedOperations _guardedOperations;
        private IBraceCompletionAggregator _sessionAggregator;
        public bool Enabled { get; }
        public bool HasActiveSessions { get; }
        public string OpeningBraces { get; }
        public string ClosingBraces { get; }

        internal BraceCompletionManager(ITextView textView, IBraceCompletionStack stack, IBraceCompletionAggregatorFactory sessionFactory, IGuardedOperations guardedOperations)
        {
            _textView = textView;
            _stack = stack;
            _sessionFactory = sessionFactory;
            _guardedOperations = guardedOperations;
            _sessionAggregator = sessionFactory.CreateAggregator();
            GetOptions();
            RegisterEvents();
        }

        public void PreTypeChar(char character, out bool handledCommand)
        {
            throw new NotImplementedException();
        }

        public void PostTypeChar(char character)
        {
            throw new NotImplementedException();
        }

        public void PreTab(out bool handledCommand)
        {
            throw new NotImplementedException();
        }

        public void PostTab()
        {
            throw new NotImplementedException();
        }

        public void PreBackspace(out bool handledCommand)
        {
            throw new NotImplementedException();
        }

        public void PostBackspace()
        {
            throw new NotImplementedException();
        }

        public void PreDelete(out bool handledCommand)
        {
            throw new NotImplementedException();
        }

        public void PostDelete()
        {
            throw new NotImplementedException();
        }

        public void PreReturn(out bool handledCommand)
        {
            throw new NotImplementedException();
        }

        public void PostReturn()
        {
            throw new NotImplementedException();
        }

        private void RegisterEvents()
        {
            throw new NotImplementedException();
        }

        private void GetOptions()
        {
            throw new NotImplementedException();
        }
    }
}
