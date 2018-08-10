using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IFormattedLineSource
    {
        double BaseIndentation { get; }

        double ColumnWidth { get; }

        TextRunProperties DefaultTextProperties { get; }

        double LineHeight { get; }

        double MaxAutoIndent { get; }

        ITextSnapshot SourceTextSnapshot { get; }

        int TabSize { get; }

        ITextAndAdornmentSequencer TextAndAdornmentSequencer { get; }

        double TextHeightAboveBaseline { get; }

        double TextHeightBelowBaseline { get; }
        ITextSnapshot TopTextSnapshot { get; }

        bool UseDisplayMode { get; }

        double WordWrapWidth { get; }

        Collection<IFormattedLine> FormatLineInVisualBuffer(ITextSnapshotLine visualLine,
            CancellationToken? cancellationToken = null);

        Collection<IFormattedLine> FormatLineInVisualBufferIfChanged(ITextSnapshotLine visualLine,
            IList<IFormattedLine> oldLines, CancellationToken? cancellationToken = null);
    }
}