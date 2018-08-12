using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
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
        [Import] internal IEditorFormatMapService EditorFormatMapService { get; set; }

        [Import] internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            return new SpacerMargin(textViewHost.TextView, TagAggregatorFactoryService, EditorFormatMapService);
        }
    }
}