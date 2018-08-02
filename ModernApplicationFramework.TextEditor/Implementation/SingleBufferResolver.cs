using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class SingleBufferResolver : ICommandingTextBufferResolver
    {
        private readonly ITextBuffer[] _textBuffer;

        public SingleBufferResolver(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            _textBuffer = new ITextBuffer[1] { textBuffer };
        }

        public IEnumerable<ITextBuffer> ResolveBuffersForCommand<TArgs>() where TArgs : EditorCommandArgs
        {
            return _textBuffer;
        }
    }
}