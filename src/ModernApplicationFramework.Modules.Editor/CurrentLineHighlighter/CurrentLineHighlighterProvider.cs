using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.CurrentLineHighlighter
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole("DOCUMENT")]
    [TextViewRole("EMBEDDED_PEEK_TEXT_VIEW")]
    [DeferCreation(OptionName = "Adornments/HighlightCurrentLine/Enable")]
    internal sealed class CurrentLineHighlighterProvider : ITextViewCreationListener
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("CurrentLineHighlighter")]
        [Order(Before = "Text")]
        [Order(Before = "SelectionAndProvisionHighlight")]
        [Order(Before = "Caret")]
        [Order(After = "Outlining")]
        [Order(Before = "TextMarker")]
        internal AdornmentLayerDefinition CurrentLineHighlighterLayer;

        [Import] internal IEditorFormatMapService EditorFormatMapService;

        public void TextViewCreated(ITextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty(() =>
                new CurrentLineHighlighter(textView, EditorFormatMapService.GetEditorFormatMap(textView)));
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("CurrentLineActiveFormat")]
        [UserVisible(true)]
        [Order(Before = "Default Priority")]
        internal sealed class CurrentLineActiveFormat : EditorFormatDefinition
        {
            public CurrentLineActiveFormat()
            {
                //TODO: localize
                DisplayName = "HighlightCurrentLine";
                //DisplayName = CurrentLineHighlighterStrings.HiglightCurrentLineActive;
                ForegroundColor = Color.FromRgb(234, 234, 242);
                BackgroundColor = Colors.Transparent;
            }
        }
    }
}