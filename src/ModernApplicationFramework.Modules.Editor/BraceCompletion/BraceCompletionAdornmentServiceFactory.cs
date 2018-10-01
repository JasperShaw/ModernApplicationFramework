using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    [Export(typeof(IBraceCompletionAdornmentServiceFactory))]
    internal class BraceCompletionAdornmentServiceFactory : IBraceCompletionAdornmentServiceFactory
    {
        [Export]
        [Name("BraceCompletion")]
        [Order(After = "DifferenceSpace")]
        public AdornmentLayerDefinition BraceCompletionAdornmentLayerDefinition;
        [Import]
        private IEditorFormatMapService _editorFormatMapService;

        public IBraceCompletionAdornmentService GetOrCreateService(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty(() =>
                (IBraceCompletionAdornmentService) new BraceCompletionAdornmentService(textView,
                    _editorFormatMapService.GetEditorFormatMap(textView)));
        }
    }
}
