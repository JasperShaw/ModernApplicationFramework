using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Text
{
    public interface ISelectionTransformer
    {
        Selection Selection { get; }

        void MoveTo(VirtualSnapshotPoint point, bool select, PositionAffinity insertionPointAffinity);

        void MoveTo(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint, VirtualSnapshotPoint insertionPoint, PositionAffinity insertionPointAffinity);

        void CapturePreferredReferencePoint();

        void CapturePreferredXReferencePoint();

        void CapturePreferredYReferencePoint();

        void PerformAction(PredefinedSelectionTransformations action);
    }
}