using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IFormattedLine : ITextViewLine, IDisposable
    {
        void SetSnapshot(ITextSnapshot visualSnapshot, ITextSnapshot editSnapshot);

        void SetLineTransform(LineTransform transform);

        void SetTop(double top);

        void SetDeltaY(double deltaY);

        void SetChange(TextViewLineChange change);

        void SetVisibleArea(Rect visibleArea);

        Visual GetOrCreateVisual();

        void RemoveVisual();

        Rect VisibleArea { get; }

        TextRunProperties GetCharacterFormatting(SnapshotPoint bufferPosition);

        ReadOnlyCollection<TextLine> TextLines { get; }
    }
}
