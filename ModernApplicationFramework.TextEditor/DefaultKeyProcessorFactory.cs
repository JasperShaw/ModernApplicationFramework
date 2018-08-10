using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IKeyProcessorProvider))]
    [Name("KeyProcessor")]
    [Order(Before = "DefaultKeyProcessor")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class DefaultKeyProcessorFactory : IKeyProcessorProvider
    {
        [Import]
        private IEditorOperationsFactoryService _editorOperationsFactoryService;

        public KeyProcessor GetAssociatedProcessor(ITextView textView)
        {
            var editorOperations = _editorOperationsFactoryService.GetEditorOperations(textView);
            return new KeyboardFilter(textView, editorOperations);
        }
    }
}