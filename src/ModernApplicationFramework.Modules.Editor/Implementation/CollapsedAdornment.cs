using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class CollapsedAdornment : OutliningCollapsedAdornmentControl
    {
        private readonly ICollapsed _collapsed;
        private readonly IOutliningManager _outliningManager;

        internal ITrackingSpan CollapsedSpan => _collapsed.Extent;

        internal bool IsToolTipSet =>
            "(placeholder until ToolTipOpening event fires -- this should never be visible)" != ToolTip;

        internal IViewPrimitives ViewPrimitives { get; }

        internal CollapsedAdornment(IViewPrimitives primitives, ICollapsed collapsed,
            IOutliningManager outliningManager)
        {
            Style = (Style) FindResource(DefaultStyleKey);
            _collapsed = collapsed;
            ViewPrimitives = primitives;
            Content = collapsed.CollapsedForm;
            ToolTip = "(placeholder until ToolTipOpening event fires -- this should never be visible)";
            _outliningManager = outliningManager;
            ToolTipOpening += HandleToolTipOpening;
            MouseLeftButtonUp += HandleMouseLeftButtonUp;
            MouseDoubleClick += HandleMouseDoubleClick;
            SetResourceReference(StyleProperty, typeof(OutliningCollapsedAdornmentControl));
            var resource = Application.Current.Resources["CollapsedAdornmentToolTipStyle"] as Style;
            if (resource == null)
                return;
            Resources.Add(typeof(ToolTip), resource);
        }

        internal void EnsureToolTip()
        {
            if (IsToolTipSet)
                return;
            ToolTip = _collapsed.CollapsedHintForm;
        }

        private void HandleMouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton != MouseButton.Left || !_collapsed.IsCollapsed)
                return;
            var advancedTextView = ViewPrimitives.View.AdvancedTextView;
            advancedTextView.Selection.Clear();
            advancedTextView.Caret.MoveTo(_collapsed.Extent.GetStartPoint(advancedTextView.TextSnapshot));
            _outliningManager.Expand(_collapsed);
            args.Handled = true;
        }

        private void HandleMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (args.ClickCount != 1)
                return;
            var advancedTextView = ViewPrimitives.View.AdvancedTextView;
            var span = _collapsed.Extent.GetSpan(advancedTextView.TextSnapshot);
            if (advancedTextView.Selection.IsEmpty)
            {
                advancedTextView.Selection.Select(span, false);
                advancedTextView.Caret.MoveTo(span.End);
                args.Handled = true;
            }
            else
            {
                if (!(advancedTextView.Selection.StreamSelectionSpan.SnapshotSpan == span))
                    return;
                args.Handled = true;
            }
        }

        private void HandleToolTipOpening(object sender, ToolTipEventArgs args)
        {
            EnsureToolTip();
        }
    }
}