using System;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public interface IFormattedLine : ITextViewLine, IDisposable
    {
        void RemoveVisual();

        Visual GetOrCreateVisual();

        void SetTop(double top);

        void SetDeltaY(double deltaY);

        void SetVisibleArea(Rect visibleArea);

        SnapshotPoint Start { get; }
        SnapshotPoint EndIncludingLineBreak { get; }
    }
}
