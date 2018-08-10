using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IFormattedLineSource
    {
        ITextSnapshot TopTextSnapshot { get; }

        ITextSnapshot SourceTextSnapshot { get; }

        ITextAndAdornmentSequencer TextAndAdornmentSequencer { get; }

        int TabSize { get; }

        double ColumnWidth { get; }

        double LineHeight { get; }

        double TextHeightAboveBaseline { get; }

        double TextHeightBelowBaseline { get; }

        double BaseIndentation { get; }

        double WordWrapWidth { get; }

        double MaxAutoIndent { get; }

        bool UseDisplayMode { get; }

        TextRunProperties DefaultTextProperties { get; }

        Collection<IFormattedLine> FormatLineInVisualBuffer(ITextSnapshotLine visualLine, CancellationToken? cancellationToken = null);

        Collection<IFormattedLine> FormatLineInVisualBufferIfChanged(ITextSnapshotLine visualLine, IList<IFormattedLine> oldLines, CancellationToken? cancellationToken = null);
    }
}