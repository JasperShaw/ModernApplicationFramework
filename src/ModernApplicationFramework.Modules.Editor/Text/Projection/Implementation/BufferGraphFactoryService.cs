using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Text.Projection.Implementation
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