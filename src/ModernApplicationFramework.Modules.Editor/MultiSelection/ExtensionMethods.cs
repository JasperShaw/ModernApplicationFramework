using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using Selection = ModernApplicationFramework.Text.Ui.Text.Selection;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    public static class ExtensionMethods
    {
        public static VirtualSnapshotPoint NormalizePoint(this ITextView view, VirtualSnapshotPoint point)
        {
            var containingBufferPosition = view.GetTextViewLineContainingBufferPosition(point.Position);
            if (point.Position >= containingBufferPosition.End)
                return new VirtualSnapshotPoint(containingBufferPosition.End, point.VirtualSpaces);
            return new VirtualSnapshotPoint(containingBufferPosition.GetTextElementSpan(point.Position).Start);
        }

        public static Selection MapToSnapshot(this Selection region, ITextSnapshot snapshot, ITextView view)
        {
            var insertionPoint = view.NormalizePoint(region.InsertionPoint.TranslateTo(snapshot));
            var virtualSnapshotPoint = view.NormalizePoint(region.ActivePoint.TranslateTo(snapshot));
            var anchorPoint = view.NormalizePoint(region.AnchorPoint.TranslateTo(snapshot));
            var activePoint = virtualSnapshotPoint;
            var insertionPointAffinity = (int)region.InsertionPointAffinity;
            return new Selection(insertionPoint, anchorPoint, activePoint, (PositionAffinity)insertionPointAffinity);
        }

        public static double MapXCoordinate(this ITextViewLine textLine, ITextView textView, double xCoordinate, ISmartIndentationService smartIndentationService, bool userSpecifiedXCoordinate)
        {
            if (textLine == null)
                throw new ArgumentNullException(nameof(textLine));
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (xCoordinate > textLine.TextRight && !textView.IsVirtualSpaceOrBoxSelectionEnabled())
            {
                var num1 = 0.0;
                if (textLine.End == textLine.Start)
                {
                    var desiredIndentation = smartIndentationService?.GetDesiredIndentation(textView, textLine.Start.GetContainingLine());
                    if (desiredIndentation.HasValue)
                    {
                        var wpfTextView = textView;
                        var num2 = wpfTextView.FormattedLineSource.ColumnWidth;
                        num1 = Math.Max(0.0, desiredIndentation.Value * num2 - textLine.TextWidth);
                        if (userSpecifiedXCoordinate && xCoordinate < textLine.TextRight + num1)
                            num1 = 0.0;
                    }
                }
                xCoordinate = textLine.TextRight + num1;
            }
            return xCoordinate;
        }

        public static bool IsVirtualSpaceOrBoxSelectionEnabled(this ITextView textView)
        {
            if (!textView.Options.IsVirtualSpaceEnabled())
                return textView.GetMultiSelectionBroker().IsBoxSelection;
            return true;
        }

        public static bool TryGetClosestTextViewLine(this ITextView textView, double yCoordinate, out ITextViewLine closestLine)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (textView.IsClosed || textView.InLayout)
            {
                closestLine = null;
                return false;
            }
            var textViewLines = textView.TextViewLines;
            if (textViewLines != null && textViewLines.Count > 0)
            {
                var textViewLine = textViewLines.GetTextViewLineContainingYCoordinate(yCoordinate);
                if (textViewLine == null)
                {
                    if (yCoordinate <= textViewLines.FirstVisibleLine.Bottom)
                        textViewLine = textViewLines.FirstVisibleLine;
                    else if (yCoordinate >= textViewLines.LastVisibleLine.Top)
                        textViewLine = textViewLines.LastVisibleLine;
                }
                closestLine = textViewLine;
                return true;
            }
            closestLine = null;
            return false;
        }
    }
}
