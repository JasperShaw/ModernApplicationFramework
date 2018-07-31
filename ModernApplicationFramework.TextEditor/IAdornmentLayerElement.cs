using System.Windows;

namespace ModernApplicationFramework.TextEditor
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