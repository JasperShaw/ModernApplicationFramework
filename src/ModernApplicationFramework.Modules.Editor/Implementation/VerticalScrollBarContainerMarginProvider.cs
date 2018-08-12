using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("VerticalScrollBarContainer")]
    [MarginContainer("RightControl")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    [GridUnitType(GridUnitType.Star)]
    internal sealed class VerticalScrollBarContainerMarginProvider : ITextViewMarginProvider
    {
        [Import] private GuardedOperations _guardedOperations;

        [Import] private TextViewMarginState _marginState;

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            return VerticalScrollBarContainerMargin.Create(textViewHost, _guardedOperations, _marginState);
        }
    }
}