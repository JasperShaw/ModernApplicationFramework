using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("VerticalScrollBarContainer")]
    [MarginContainer("RightControl")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    [GridUnitType(GridUnitType.Star)]
    internal sealed class VerticalScrollBarContainerMarginProvider : ITextViewMarginProvider
    {
        [Import]
        private TextViewMarginState _marginState;
        [Import]
        private GuardedOperations _guardedOperations;

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            return VerticalScrollBarContainerMargin.Create(textViewHost, _guardedOperations, _marginState);
        }
    }
}
