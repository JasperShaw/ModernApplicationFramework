using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole("EDITABLE")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class BraceCompletionManagerFactory : ITextViewCreationListener
    {
        [Import]
        private IBraceCompletionAdornmentServiceFactory _adornmentServiceFactory;
        [Import]
        private IBraceCompletionAggregatorFactory _aggregatorFactory;
        [Import]
        private GuardedOperations _guardedOperations;

        public void TextViewCreated(ITextView textView)
        {
            textView.Properties.AddProperty("BraceCompletionManager",
                new BraceCompletionManager(textView,
                    new BraceCompletionStack(textView, _adornmentServiceFactory,
                        _guardedOperations), _aggregatorFactory, _guardedOperations));
        }
    }
}
