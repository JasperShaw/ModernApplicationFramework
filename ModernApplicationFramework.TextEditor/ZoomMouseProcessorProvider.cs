using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IMouseProcessorProvider))]
    [Name("Zoom")]
    [ContentType("Text")]
    [TextViewRole("ZOOMABLE")]
    [TextViewRole("EMBEDDED_PEEK_TEXT_VIEW")]
    internal sealed class ZoomMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import]
        internal IEditorOperationsFactoryService EditorOperationsFactoryService;

        public IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView)
        {
            if (wpfTextView == null)
                throw new ArgumentNullException(nameof(wpfTextView));
            return wpfTextView.Properties.GetOrCreateSingletonProperty<IMouseProcessor>(() => new ZoomMouseProcessor(wpfTextView, this));
        }
    }
}