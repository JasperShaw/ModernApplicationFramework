using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.IntraTextAdornmentSupport
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole("STRUCTURED")]
    internal sealed class HiddenRegionManagerFactory : ITextViewCreationListener
    {
        [Import]
        public IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService { get; set; }

        public void TextViewCreated(ITextView textView)
        {
            if (!textView.TextViewModel.Properties.TryGetProperty("IntraTextAdornmentBuffer", out IElisionBuffer property))
                return;
            var tagAggregator = ViewTagAggregatorFactoryService.CreateTagAggregator<IElisionTag>(textView);
            var hiddenRegionManager = new HiddenRegionManager(property, tagAggregator);
            textView.Closed += (_param1, _param2) => hiddenRegionManager.Dispose();
        }
    }
}
