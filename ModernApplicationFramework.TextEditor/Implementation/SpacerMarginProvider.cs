using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("Spacer")]
    [Order(After = "LineNumber")]
    [MarginContainer("LeftSelection")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    [DeferCreation(OptionName = "TextViewHost/SelectionMargin")]
    internal sealed class SpacerMarginProvider : ITextViewMarginProvider
    {
        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; set; }

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            return new SpacerMargin(textViewHost.TextView, TagAggregatorFactoryService, EditorFormatMapService);
        }
    }
}