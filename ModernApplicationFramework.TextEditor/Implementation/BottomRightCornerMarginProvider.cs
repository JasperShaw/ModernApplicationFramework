using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("BottomRightCornerMargin")]
    [Order]
    [MarginContainer("BottomRightCorner")]
    [ContentType("text")]
    [GridCellLength(1.0)]
    [GridUnitType(GridUnitType.Star)]
    [TextViewRole("INTERACTIVE")]
    internal sealed class BottomRightCornerMarginProvider : ITextViewMarginProvider
    {
        public ITextViewMargin CreateMargin(ITextViewHost wpfTextViewHost, ITextViewMargin marginContainer)
        {
            return new BottomRightCornerMargin();
        }
    }
}