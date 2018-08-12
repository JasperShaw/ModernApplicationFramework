using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
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