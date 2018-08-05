using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("HorizontalScrollBar")]
    [MarginContainer("HorizontalScrollBarContainer")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class HorizontalScrollBarMarginProvider : ITextViewMarginProvider
    {
        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            return new HorizontalScrollBarMargin(textViewHost.TextView);
        }
    }
}