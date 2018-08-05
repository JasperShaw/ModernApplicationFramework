using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("RightControl")]
    [MarginContainer("Right")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class RightControlMarginProvider : ITextViewMarginProvider
    {
        [Import]
        private TextViewMarginState _marginState;
        [Import]
        private GuardedOperations _guardedOperations;

        public ITextViewMargin CreateMargin(ITextViewHost wpfTextViewHost, ITextViewMargin marginContainer)
        {
            return ContainerMargin.Create("RightControl", Orientation.Horizontal, wpfTextViewHost, _guardedOperations, _marginState);
        }
    }
}