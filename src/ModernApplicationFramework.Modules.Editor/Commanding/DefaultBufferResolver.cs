using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    internal class DefaultBufferResolver : ICommandingTextBufferResolver
    {
        private readonly ITextView _textView;

        public DefaultBufferResolver(ITextView textView)
        {
            var textView1 = textView;
            _textView = textView1 ?? throw new ArgumentNullException(nameof(textView));
        }

        public IEnumerable<ITextBuffer> ResolveBuffersForCommand<TArgs>() where TArgs : EditorCommandArgs
        {
            var mappingPoint = _textView.BufferGraph.CreateMappingPoint(_textView.Caret.Position.BufferPosition,
                PointTrackingMode.Negative);
            return _textView.BufferGraph.GetTextBuffers(b =>
                mappingPoint.GetPoint(b, PositionAffinity.Successor).HasValue);
        }
    }
}