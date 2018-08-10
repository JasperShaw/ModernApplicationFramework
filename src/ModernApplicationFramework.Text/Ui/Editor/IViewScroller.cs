using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IViewScroller
    {
        void ScrollViewportVerticallyByPixels(double distanceToScroll);

        void ScrollViewportVerticallyByLine(ScrollDirection direction);

        void ScrollViewportVerticallyByLines(ScrollDirection direction, int count);

        bool ScrollViewportVerticallyByPage(ScrollDirection direction);

        void ScrollViewportHorizontallyByPixels(double distanceToScroll);

        void EnsureSpanVisible(SnapshotSpan span);

        void EnsureSpanVisible(SnapshotSpan span, EnsureSpanVisibleOptions options);

        void EnsureSpanVisible(VirtualSnapshotSpan span, EnsureSpanVisibleOptions options);
    }
}