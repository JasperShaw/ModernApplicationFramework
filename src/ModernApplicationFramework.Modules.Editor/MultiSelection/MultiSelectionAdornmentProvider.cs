using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("any")]
    [TextViewRole("INTERACTIVE")]
    internal class MultiSelectionAdornmentProvider : ITextViewCreationListener
    {
        [Export]
        [Name("Caret")]
        [Order(After = "Text")]
        internal AdornmentLayerDefinition CaretAdornmentLayer;

        [Import]
        internal IMultiSelectionBrokerFactory MultiSelectionBrokerFactory;
        [Import]
        internal IEditorFormatMapService EditorFormatMapService;
        [Import]
        internal IClassificationFormatMapService ClassificationFormatMappingService;

        public void TextViewCreated(ITextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty(() => new CaretAdornmentLayer(this, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new SelectionAdornmentLayer(this, textView));
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Caret (Primary)")]
        [UserVisible(true)]
        internal sealed class PrimaryCaretProperties : EditorFormatDefinition
        {
            public PrimaryCaretProperties()
            {
                BackgroundCustomizable = false;
                ForegroundColor = Color.FromRgb(243, 5, 6);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Caret (Secondary)")]
        [UserVisible(true)]
        internal sealed class SecondaryCaretProperties : EditorFormatDefinition
        {
            public SecondaryCaretProperties()
            {
                BackgroundCustomizable = false;
                ForegroundColor = Color.FromRgb(0, 151, 251);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Selected Text")]
        [UserVisible(true)]
        internal sealed class ActiveSelectionProperties : EditorFormatDefinition
        {
            public ActiveSelectionProperties()
            {
                BackgroundColor = SystemColors.HighlightColor;
                DisplayName = "Selected Text";
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Inactive Selected Text")]
        [UserVisible(true)]
        internal sealed class InactiveSelectionProperties : EditorFormatDefinition
        {
            public InactiveSelectionProperties()
            {
                BackgroundColor = SystemColors.GrayTextColor;
                DisplayName = "Inactive Selected Text";
            }
        }
    }
}
