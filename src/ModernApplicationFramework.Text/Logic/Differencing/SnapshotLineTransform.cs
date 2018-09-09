using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Differencing
{
    public delegate string SnapshotLineTransform(ITextSnapshotLine line, string currentText);
}