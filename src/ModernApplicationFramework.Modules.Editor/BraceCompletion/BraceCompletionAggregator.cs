using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal sealed class BraceCompletionAggregator : IBraceCompletionAggregator
    {
        private BraceCompletionAggregatorFactory _factory;

        public string OpeningBraces { get; }
        public string ClosingBraces { get; }

        public BraceCompletionAggregator(BraceCompletionAggregatorFactory factory)
        {
            _factory = factory;
            //Init();
        }

        public bool IsSupportedContentType(IContentType contentType, char openingBrace)
        {
            throw new NotImplementedException();
        }

        public bool TryCreateSession(ITextView textView, SnapshotPoint openingPoint, char openingBrace,
            out IBraceCompletionSession session)
        {
            throw new NotImplementedException();
        }
    }
}
