using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Differencing;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("deltaSideBySideVerticalScrollBar")]
    [Replaces("VerticalScrollBar")]
    [MarginContainer("VerticalScrollBarContainer")]
    [ContentType("text")]
    [TextViewRole("RIGHTDIFF")]
    internal sealed class SideBySideVerticalScrollBarMarginProvider : ITextViewMarginProvider
    {
        public const string MarginName = "deltaSideBySideVerticalScrollBar";

        [Import]
        internal IScrollMapFactoryService ScrollMapFactoryService { get; set; }

        [Import(typeof(PerformanceBlockMarker))]
        internal PerformanceBlockMarker PerformanceBlockMarker { get; set; }

        public ITextViewMargin CreateMargin(ITextViewHost wpfTextViewHost, ITextViewMargin marginContainer)
        {
            if (wpfTextViewHost == null)
                throw new ArgumentNullException(nameof(wpfTextViewHost));
            IDifferenceTextViewModel textViewModel = wpfTextViewHost.TextView.TextViewModel as IDifferenceTextViewModel;
            if (textViewModel == null)
                return null;
            return new SideBySideVerticalScrollBarMargin(wpfTextViewHost.TextView, new SideBySideScrollMap(textViewModel.Viewer, this.ScrollMapFactoryService.Create(wpfTextViewHost.TextView, false)), this.PerformanceBlockMarker);
        }
    }
}