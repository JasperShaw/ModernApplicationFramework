using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.AdornmentLibrary.VisibleWhitespace
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeferCreation(OptionName = "TextView/UseVisibleWhitespace")]
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("Text")]
    [TextViewRole("INTERACTIVE")]
    [TextViewRole("ENHANCED_SCROLLBAR_PREVIEW")]
    [TextViewRole("EMBEDDED_PEEK_TEXT_VIEW")]
    internal sealed class VisibleWhitespaceFactory : ITextViewCreationListener
    {
        [Export]
        [Name("VisibleWhitespace")]
        [Order(After = "Text", Before = "Caret")]
        internal AdornmentLayerDefinition VisibleWhitespaceLayer;

        [Import] private IEditorFormatMapService _editorFormatMappingService;

        [Import] private IEditorOptionsFactoryService _editorOptionsFactory;

        public void TextViewCreated(ITextView textView)
        {
            CreateVisualProvider(textView, _editorOptionsFactory.GetOptions(textView));
        }

        internal VisibleWhitespaceVisualProvider CreateVisualProvider(ITextView textView, IEditorOptions options)
        {
            return new VisibleWhitespaceVisualProvider(textView, options,
                _editorFormatMappingService.GetEditorFormatMap(textView));
        }
    }
}