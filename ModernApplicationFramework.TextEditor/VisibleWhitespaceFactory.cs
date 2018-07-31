using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeferCreation(OptionName = "TextView/UseVisibleWhitespace")]
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("Text")]
    [TextViewRole("DOCUMENT")]
    [TextViewRole("ENHANCED_SCROLLBAR_PREVIEW")]
    [TextViewRole("EMBEDDED_PEEK_TEXT_VIEW")]
    internal sealed class VisibleWhitespaceFactory : ITextViewCreationListener
    {
        [Import]
        private IEditorOptionsFactoryService _editorOptionsFactory;
        [Import]
        private IEditorFormatMapService _editorFormatMappingService;
        [Export]
        [Name("VisibleWhitespace")]
        [Order(After = "Text", Before = "Caret")]
        internal AdornmentLayerDefinition VisibleWhitespaceLayer;

        public void TextViewCreated(ITextView textView)
        {
            CreateVisualProvider(textView, _editorOptionsFactory.GetOptions(textView));
        }

        internal VisibleWhitespaceVisualProvider CreateVisualProvider(ITextView textView, IEditorOptions options)
        {
            return new VisibleWhitespaceVisualProvider(textView, options, _editorFormatMappingService.GetEditorFormatMap(textView));
        }
    }
}