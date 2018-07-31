using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IMouseProcessorProvider))]
    [Name("WordSelection")]
    [ContentType("Text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class SelectionMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import]
        internal IEditorOperationsFactoryService EditorOperationsProvider { get; set; }

        [Import]
        internal IEditorPrimitivesFactoryService EditorPrimitivesFactory { get; set; }

        public IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView)
        {
            return new SelectionMouseProcessor(wpfTextView, EditorOperationsProvider, EditorPrimitivesFactory);
        }
    }
}