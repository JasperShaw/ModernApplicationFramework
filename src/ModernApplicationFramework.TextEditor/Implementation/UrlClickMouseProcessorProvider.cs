using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(IMouseProcessorProvider))]
    [TextViewRole("INTERACTIVE")]
    [ContentType("text")]
    [Name("UrlClickMouseProcessor")]
    [Order(Before = "MouseProcessor")]
    internal sealed class UrlClickMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import]
        internal IViewTagAggregatorFactoryService AggregatorFactory;
        [Import]
        internal IEditorAdaptersFactoryService AdaptersFactory;

        public IMouseProcessor GetAssociatedProcessor(ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return new UrlClickMouseHandler(textView, AdaptersFactory,
                AggregatorFactory.CreateTagAggregator<IUrlTag>(textView),
                CtrlKeyStateTracker.GetStateTrackerForView(textView));
        }
    }
}