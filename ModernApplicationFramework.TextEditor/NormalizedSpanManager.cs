using System;
using System.Collections.Generic;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.TextEditor.Text.Formatting;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class NormalizedSpanManager
    {
        private readonly ITextSnapshot _visualTextSnapshot;
        private readonly NormalizedSpanTextSource _textSource;
        private readonly IBufferGraph _bufferGraph;
        private readonly BufferPositionToTokenIndexMap _bufferToTokenMap;
        private readonly int _lineBreakLength;
        private readonly int _endTokenIndex;
        private readonly TextInfoCache _textInfoCache;
        private readonly double _defaultSpaceWidth;
        private readonly ViewWrapProperties _viewWrapProperties;
        private int _currentTokenIndex;

        public NormalizedSpanManager(ITextSnapshot visualTextSnapshot, ITextSnapshot sourceTextSnapshot, IBufferGraph bufferGraph, IList<NormalizedSpan> normalizedSpans, int lineBreakLength, ViewWrapProperties viewWrapProperties, TextInfoCache textInfoCache, double defaultSpaceWidth)
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

        public IFormattedLine FormatLine(double indentation, double maxTextWidth, TextParagraphProperties paragraphProperties)
        {
            if (maxTextWidth > 0.0)
                maxTextWidth += 0.5;
            bool isFirstTextViewLineForSnapshotLine = _currentTokenIndex == 0;
            int currentTokenIndex = _currentTokenIndex;
            double num = 0.0;
            TextLineBreak lastLineBreak = null;
            FrugalList<TextLineData> frugalList = new FrugalList<TextLineData>();
            do
            {
                _textSource.TextLineStartIndex = maxTextWidth == 0.0 ? _currentTokenIndex : int.MaxValue;
                TextLine textLine = FormatLine(_endTokenIndex, maxTextWidth, paragraphProperties, lastLineBreak, out var endOfLineTokenIndex);
                if (maxTextWidth > 0.0)
                {
                    int nextTokenIndex = _bufferToTokenMap.GetNextTokenIndex(_currentTokenIndex, _endTokenIndex, _lineBreakLength);
                    if (num + textLine.WidthIncludingTrailingWhitespace > maxTextWidth)
                    {
                        CharacterHit characterHitFromDistance = textLine.GetCharacterHitFromDistance(maxTextWidth - num);
                        int forcedLineBreak = Math.Max(nextTokenIndex, characterHitFromDistance.FirstCharacterIndex);
                        if (forcedLineBreak != endOfLineTokenIndex)
                            textLine = FormatLine(forcedLineBreak, 0.0, paragraphProperties, lastLineBreak, out endOfLineTokenIndex);
                    }
                    else if (endOfLineTokenIndex < _endTokenIndex)
                    {
                        if (_textSource.IsTab(endOfLineTokenIndex) && GetNextTabStop(textLine.WidthIncludingTrailingWhitespace, paragraphProperties) < maxTextWidth)
                            textLine = FormatLine(endOfLineTokenIndex + 1, 0.0, paragraphProperties, lastLineBreak, out endOfLineTokenIndex);
                        else if (endOfLineTokenIndex < nextTokenIndex)
                            textLine = FormatLine(nextTokenIndex, 0.0, paragraphProperties, lastLineBreak, out endOfLineTokenIndex);
                    }
                }
                lastLineBreak = textLine.GetTextLineBreak();
                frugalList.Add(new TextLineData(Span.FromBounds(_currentTokenIndex, endOfLineTokenIndex), frugalList.Count, indentation + num, textLine));
                num += textLine.WidthIncludingTrailingWhitespace;
                _currentTokenIndex = endOfLineTokenIndex;
                MoreLinesToFormat = _currentTokenIndex < _endTokenIndex;
            }
            while (MoreLinesToFormat && maxTextWidth == 0.0);

            return new FormattedLine(indentation, frugalList, _bufferGraph,
                _visualTextSnapshot, _textSource, _bufferToTokenMap, currentTokenIndex, _currentTokenIndex,
                _currentTokenIndex == _endTokenIndex ? _lineBreakLength : 0, isFirstTextViewLineForSnapshotLine,
                _currentTokenIndex == _endTokenIndex,
                !MoreLinesToFormat || maxTextWidth == 0.0 ? null : _viewWrapProperties,
                _textInfoCache, _defaultSpaceWidth);
        }

        internal static double GetNextTabStop(double position, TextParagraphProperties paragraphProperties)
        {
            double num = 0.0;
            if (paragraphProperties.Tabs != null)
            {
                foreach (TextTabProperties tab in paragraphProperties.Tabs)
                {
                    num = tab.Location;
                    if (num > position)
                        return num;
                }
            }
            return num + (Math.Floor((position - num) / paragraphProperties.DefaultIncrementalTab) + 1.0) * paragraphProperties.DefaultIncrementalTab;
        }

        private TextLine FormatLine(int forcedLineBreak, double maxWidth, TextParagraphProperties paragraphProperties, TextLineBreak lastLineBreak, out int endOfLineTokenIndex)
        {
            _textSource.ForcedLineBreakIndex = forcedLineBreak;
            TextLine line = _textInfoCache.TextFormatter.FormatLine(_textSource, _currentTokenIndex, maxWidth, paragraphProperties, lastLineBreak);
            endOfLineTokenIndex = _currentTokenIndex + GetTextLineLength(line);
            return line;
        }

        public bool MoreLinesToFormat { get; private set; } = true;

        public SnapshotPoint CurrentSnapshotPoint => new SnapshotPoint(_bufferToTokenMap.SourceTextSnapshot, _bufferToTokenMap.GetNextBufferPositionFromTokenIndex(_currentTokenIndex));

        private static int GetTextLineLength(TextLine line)
        {
            int length = line.Length;
            IList<TextSpan<TextRun>> textRunSpans = line.GetTextRunSpans();
            if (textRunSpans.Count > 0 && textRunSpans[textRunSpans.Count - 1].Value == NormalizedSpanTextSource.LineBreak)
                length -= NormalizedSpanTextSource.LineBreak.Length;
            return length;
        }
    }
}