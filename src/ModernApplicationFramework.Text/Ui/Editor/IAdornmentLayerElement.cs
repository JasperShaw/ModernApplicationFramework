using System.Windows;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IAdornmentLayerElement
    {
        SnapshotSpan? VisualSpan { get; }

        AdornmentPositioningBehavior Behavior { get; }

        UIElement Adornment { get; }

        object Tag { get; }

        AdornmentRemovedCallback RemovedCallback { get; }
    }
}