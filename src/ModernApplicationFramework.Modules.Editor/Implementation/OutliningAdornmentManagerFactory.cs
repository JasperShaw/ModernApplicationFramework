using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("any")]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class OutliningAdornmentManagerFactory : IViewTaggerProvider
    {
        [Import]
        internal IOutliningManagerService OutliningManagerService { get; set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; set; }

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMapService { get; set; }

        [Import]
        internal IEditorPrimitivesFactoryService EditorPrimitivesFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (buffer != textView.TextViewModel.EditBuffer)
                return null;
            var manager = OutliningManagerService.GetOutliningManager(textView);
            if (manager == null)
                return null;
            var editorFormatMap = EditorFormatMapService.GetEditorFormatMap(textView);
            var classificationFormatMap = ClassificationFormatMapService.GetClassificationFormatMap(textView);
            return textView.Properties.GetOrCreateSingletonProperty(() => new CollapsedAdornmentProvider(textView, manager, editorFormatMap, classificationFormatMap, EditorPrimitivesFactoryService.GetViewPrimitives(textView))) as ITagger<T>;
        }
    }
}
