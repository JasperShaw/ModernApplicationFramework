using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IKeyProcessorProvider))]
    [Name("DefaultKeyProcessor")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class PrimitiveKeyProcessorFactory : IKeyProcessorProvider
    {
        [Import]
        private IEditorOperationsFactoryService _editorOperationsFactoryService;

        public KeyProcessor GetAssociatedProcessor(ITextView textView)
        {
            var editorOperations = _editorOperationsFactoryService.GetEditorOperations(textView);
            return new PrimitiveKeyboardFilter(textView, editorOperations);
        }
    }
}