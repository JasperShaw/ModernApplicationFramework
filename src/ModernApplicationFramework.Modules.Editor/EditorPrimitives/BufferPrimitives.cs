using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    internal sealed class BufferPrimitives : IBufferPrimitives
    {
        public BufferPrimitives(ITextBuffer textBuffer, IBufferPrimitivesFactoryService bufferPrimitivesFactory)
        {
            Buffer = bufferPrimitivesFactory.CreateTextBuffer(textBuffer);
        }

        public PrimitiveTextBuffer Buffer { get; }
    }
}