using System;
using System.Collections.Generic;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class NormalizedSpanManager
    {
        private readonly IBufferGraph _bufferGraph;
        private readonly BufferPositionToTokenIndexMap _bufferToTokenMap;
        private readonly double _defaultSpaceWidth;
        private readonly int _endTokenIndex;
        private readonly int _lineBreakLength;
        private readonly TextInfoCache _textInfoCache;
        private readonly NormalizedSpanTextSource _textSource;
        private readonly ViewWrapProperties _viewWrapProperties;
        private readonly ITextSnapshot _visualTextSnapshot;
        private int _currentTokenIndex;

        public SnapshotPoint CurrentSnapshotPoint => new SnapshotPoint(_bufferToTokenMap.SourceTextSnapshot,
            _bufferToTokenMap.GetNextBufferPositionFromTokenIndex(_currentTokenIndex));

        public bool MoreLinesToFormat { get; private set; } = true;

        public NormalizedSpanManager(ITextSnapshot visualTextSnapshot, ITextSnapshot sourceTextSnapshot,
            IBufferGraph bufferGraph, IList<NormalizedSpan> normalizedSpans, int lineBreakLength,
            ViewWrapProperties viewWrapProperties, TextInfoCache textInfoCache, double defaultSpaceWidth)
        {
            _textSource = new NormalizedSpanTextSource(normalizedSpans);
            _visualTextSnapshot = visualTextSnapshot;
            _bufferGraph = bufferGraph;
            _bufferToTokenMap = new BufferPositionToTokenIndexMap(sourceTextSnapshot, normalizedSpans);
            _lineBreakLength = lineBreakLength;
            _viewWrapProperties = viewWrapProperties;
            _textInfoCache = textInfoCache;
            _defaultSpaceWidth = defaultSpaceWidth;
            _endTokenIndex = _textSource.NormalizedSpans[_textSource.NormalizedSpans.Count - 1].TokenSpan.End;
        }

        public IFormattedLine FormatLine(double indentation, double maxTextWidth,
            TextParagraphProperties paragraphProperties)
        {
            if (maxTextWidth > 0.0)
                maxTextWidth += 0.5;
            var isFirstTextViewLineForSnapshotLine = _currentTokenIndex == 0;
            var currentTokenIndex = _currentTokenIndex;
            var num = 0.0;
            TextLineBreak lastLineBreak = null;
            var frugalList = new FrugalList<TextLineData>();
            do
            {
                _textSource.TextLineStartIndex = maxTextWidth == 0.0 ? _currentTokenIndex : int.MaxValue;
                var textLine = FormatLine(_endTokenIndex, maxTextWidth, paragraphProperties, lastLineBreak,
                    out var endOfLineTokenIndex);
                if (maxTextWidth > 0.0)
                {
                    var nextTokenIndex =
                        _bufferToTokenMap.GetNextTokenIndex(_currentTokenIndex, _endTokenIndex, _lineBreakLength);
                    if (num + textLine.WidthIncludingTrailingWhitespace > maxTextWidth)
                    {
                        var characterHitFromDistance = textLine.GetCharacterHitFromDistance(maxTextWidth - num);
                        var forcedLineBreak = Math.Max(nextTokenIndex, characterHitFromDistance.FirstCharacterIndex);
                        if (forcedLineBreak != endOfLineTokenIndex)
                            textLine = FormatLine(forcedLineBreak, 0.0, paragraphProperties, lastLineBreak,
                                out endOfLineTokenIndex);
                    }
                    else if (endOfLineTokenIndex < _endTokenIndex)
                    {
                        if (_textSource.IsTab(endOfLineTokenIndex) &&
                            GetNextTabStop(textLine.WidthIncludingTrailingWhitespace, paragraphProperties) <
                            maxTextWidth)
                            textLine = FormatLine(endOfLineTokenIndex + 1, 0.0, paragraphProperties, lastLineBreak,
                                out endOfLineTokenIndex);
                        else if (endOfLineTokenIndex < nextTokenIndex)
                            textLine = FormatLine(nextTokenIndex, 0.0, paragraphProperties, lastLineBreak,
                                out endOfLineTokenIndex);
                    }
                }

                lastLineBreak = textLine.GetTextLineBreak();
                frugalList.Add(new TextLineData(Span.FromBounds(_currentTokenIndex, endOfLineTokenIndex),
                    frugalList.Count, indentation + num, textLine));
                num += textLine.WidthIncludingTrailingWhitespace;
                _currentTokenIndex = endOfLineTokenIndex;
                MoreLinesToFormat = _currentTokenIndex < _endTokenIndex;
            } while (MoreLinesToFormat && maxTextWidth == 0.0);

            return new FormattedLine(indentation, frugalList, _bufferGraph,
                _visualTextSnapshot, _textSource, _bufferToTokenMap, currentTokenIndex, _currentTokenIndex,
                _currentTokenIndex == _endTokenIndex ? _lineBreakLength : 0, isFirstTextViewLineForSnapshotLine,
                _currentTokenIndex == _endTokenIndex,
                !MoreLinesToFormat || maxTextWidth == 0.0 ? null : _viewWrapProperties,
                _textInfoCache, _defaultSpaceWidth);
        }

        internal static double GetNextTabStop(double position, TextParagraphProperties paragraphProperties)
        {
            var num = 0.0;
            if (paragraphProperties.Tabs != null)
                foreach (var tab in paragraphProperties.Tabs)
                {
                    num = tab.Location;
                    if (num > position)
                        return num;
                }

            return num + (Math.Floor((position - num) / paragraphProperties.DefaultIncrementalTab) + 1.0) *
                   paragraphProperties.DefaultIncrementalTab;
        }

        private static int GetTextLineLength(TextLine line)
        {
            var length = line.Length;
            var textRunSpans = line.GetTextRunSpans();
            if (textRunSpans.Count > 0 &&
                textRunSpans[textRunSpans.Count - 1].Value == NormalizedSpanTextSource.LineBreak)
                length -= NormalizedSpanTextSource.LineBreak.Length;
            return length;
        }

        private TextLine FormatLine(int forcedLineBreak, double maxWidth, TextParagraphProperties paragraphProperties,
            TextLineBreak lastLineBreak, out int endOfLineTokenIndex)
        {
            _textSource.ForcedLineBreakIndex = forcedLineBreak;
            var line = _textInfoCache.TextFormatter.FormatLine(_textSource, _currentTokenIndex, maxWidth,
                paragraphProperties, lastLineBreak);
            endOfLineTokenIndex = _currentTokenIndex + GetTextLineLength(line);
            return line;
        }
    }
}