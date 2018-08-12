using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;

namespace ModernApplicationFramework.Modules.Editor.Commanding
{
    internal class SingleBufferResolver : ICommandingTextBufferResolver
    {
        private readonly ITextBuffer[] _textBuffer;

        public SingleBufferResolver(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            _textBuffer = new ITextBuffer[1] {textBuffer};
        }

        public IEnumerable<ITextBuffer> ResolveBuffersForCommand<TArgs>() where TArgs : EditorCommandArgs
        {
            return _textBuffer;
        }
    }
}