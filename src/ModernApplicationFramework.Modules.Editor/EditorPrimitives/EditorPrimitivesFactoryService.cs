using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    [Export(typeof(IEditorPrimitivesFactoryService))]
    internal sealed class EditorPrimitivesFactoryService : IEditorPrimitivesFactoryService
    {
        [Import] internal IBufferPrimitivesFactoryService BufferPrimitivesFactory { get; set; }

        [Import] internal IViewPrimitivesFactoryService ViewPrimitivesFactory { get; set; }

        public IBufferPrimitives GetBufferPrimitives(ITextBuffer textBuffer)
        {
            return new BufferPrimitives(textBuffer, BufferPrimitivesFactory);
        }

        public IViewPrimitives GetViewPrimitives(ITextView textView)
        {
            return new ViewPrimitives(textView, ViewPrimitivesFactory);
        }
    }
}