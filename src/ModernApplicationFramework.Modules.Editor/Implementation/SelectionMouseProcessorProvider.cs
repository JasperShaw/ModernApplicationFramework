using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(IMouseProcessorProvider))]
    [Name("WordSelection")]
    [ContentType("Text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class SelectionMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import] internal IEditorOperationsFactoryService EditorOperationsProvider { get; set; }

        [Import] internal IEditorPrimitivesFactoryService EditorPrimitivesFactory { get; set; }

        public IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView)
        {
            return new SelectionMouseProcessor(wpfTextView, EditorOperationsProvider, EditorPrimitivesFactory);
        }
    }
}