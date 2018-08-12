using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.HighContrast
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("any")]
    [TagType(typeof(ClassificationTag))]
    [TextViewRole("INTERACTIVE")]
    internal class HighContrastSelectionTaggerProvider : IViewTaggerProvider
    {
        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer != buffer)
                return null;
            return new HighContrastSelectionTagger(textView, _classificationRegistry, EditorFormatMapService) as ITagger<T>;
        }
    }
}