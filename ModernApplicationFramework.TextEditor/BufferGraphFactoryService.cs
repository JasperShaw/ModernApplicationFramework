using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IBufferGraphFactoryService))]
    internal sealed class BufferGraphFactoryService : IBufferGraphFactoryService
    {
        [Import]
        internal GuardedOperations GuardedOperations;

        public IBufferGraph CreateBufferGraph(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new BufferGraph(textBuffer, GuardedOperations));
        }
    }
}