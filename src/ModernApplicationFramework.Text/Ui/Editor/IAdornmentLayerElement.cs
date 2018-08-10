using System.Windows;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IAdornmentLayerElement
    {
        UIElement Adornment { get; }

        AdornmentPositioningBehavior Behavior { get; }

        AdornmentRemovedCallback RemovedCallback { get; }

        object Tag { get; }
        SnapshotSpan? VisualSpan { get; }
    }
}