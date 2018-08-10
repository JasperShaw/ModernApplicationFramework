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
        ReadOnlyCollection<TextLine> TextLines { get; }

        Rect VisibleArea { get; }

        TextRunProperties GetCharacterFormatting(SnapshotPoint bufferPosition);

        Visual GetOrCreateVisual();

        void RemoveVisual();

        void SetChange(TextViewLineChange change);

        void SetDeltaY(double deltaY);

        void SetLineTransform(LineTransform transform);
        void SetSnapshot(ITextSnapshot visualSnapshot, ITextSnapshot editSnapshot);

        void SetTop(double top);

        void SetVisibleArea(Rect visibleArea);
    }
}