using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IViewSynchronizationManager
    {
        ITextView GetSubordinateView(ITextView masterView);

        bool TryGetAnchorPointInSubordinateView(SnapshotPoint anchorPoint, out SnapshotPoint correspondingAnchorPoint);

        SnapshotPoint GetAnchorPointAboveInSubordinateView(SnapshotPoint anchorPoint);

        void WhichPairedLinesShouldBeDisplayed(SnapshotPoint masterAnchorPoint, SnapshotPoint subordinateAnchorPoint, out bool layoutMaster, out bool layoutSubordinate, bool goingUp);
    }
}