using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    [Export(typeof(IMouseProcessorProvider))]
    [TextViewRole("INTERACTIVE")]
    [ContentType("any")]
    [Name("MultiSelectionMouseHandlerProvider")]
    [Order(After = "DragDrop")]
    [Order(After = "WordSelection")]
    internal class MultiSelectionMouseHandlerProvider : IMouseProcessorProvider
    {
        public const string ProviderName = "MultiSelectionMouseHandlerProvider";

        [Import]
        internal IMultiSelectionBrokerFactory MultiSelectionBrokerFactory { get; set; }

        [Import]
        internal ISmartIndentationService SmartIndentationService { get; set; }

        public IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView)
        {
            return wpfTextView.Properties.GetOrCreateSingletonProperty(() => new MultiSelectionMouseHandler(this, wpfTextView));
        }
    }
}
