using System;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal static class OverviewExtensions
    {
        internal static Brush GetBrush(this IEditorFormatMap formatMap, string key, string resource)
        {
            var properties = formatMap.GetProperties(key);
            if (properties.Contains(resource))
                return properties[resource] as Brush;
            return null;
        }

        internal static void ScrollViewToYCoordinate(this ITextView textView, IVerticalScrollBar verticalScrollBar, IOutliningManager outliningManager, double y)
        {
            var num1 = verticalScrollBar.TrackSpanBottom - verticalScrollBar.ThumbHeight;
            if (y < num1)
            {
                var positionOfYcoordinate = verticalScrollBar.GetBufferPositionOfYCoordinate(y);
                ExpandOutliningRegions(outliningManager, positionOfYcoordinate);
                textView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(positionOfYcoordinate, 0), EnsureSpanVisibleOptions.AlwaysCenter);
            }
            else
            {
                y = Math.Min(y, num1 + verticalScrollBar.ThumbHeight / 2.0);
                var num2 = (y - num1) / verticalScrollBar.ThumbHeight;
                var verticalDistance = textView.ViewportHeight * (0.5 - num2);
                var snapshotPoint = new SnapshotPoint(textView.TextSnapshot, textView.TextSnapshot.Length);
                ExpandOutliningRegions(outliningManager, snapshotPoint);
                textView.DisplayTextLineContainingBufferPosition(snapshotPoint, verticalDistance, ViewRelativePosition.Top);
            }
        }

        private static void ExpandOutliningRegions(IOutliningManager outliningManager, SnapshotPoint position)
        {
            outliningManager?.ExpandAll(new SnapshotSpan(position, 0), collapsible =>
            {
                Span span = collapsible.Extent.GetSpan(position.Snapshot);
                if (position > span.Start)
                    return position < span.End;
                return false;
            });
        }
    }
}