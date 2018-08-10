using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IMouseProcessorProvider))]
    [Name("MouseProcessor")]
    [Order(Before = "WordSelection")]
    [ContentType("Text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class MouseProcessorProvider : IMouseProcessorProvider
    {
        [Import]
        internal IEditorPrimitivesFactoryService EditorPrimitivesFactoryService;

        public IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView)
        {
            return new MouseProcessor(wpfTextView, EditorPrimitivesFactoryService);
        }
    }
}