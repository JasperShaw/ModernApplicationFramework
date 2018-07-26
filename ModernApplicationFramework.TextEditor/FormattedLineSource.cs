using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Text.Formatting;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class FormattedLineSource : IFormattedLineSource
    {
        private readonly TextInfoCache.FontInfo _defaultFontInfo;
        internal IAccurateClassifier AggregateClassifier;
        private readonly IClassificationFormatMap _classificationFormatMap;
        internal ViewWrapProperties ViewWrapProperties;
        private readonly TextParagraphProperties _defaultParagraphProperties;
        private bool _inFormat;
        private readonly TextInfoCache _textInfoCache;
        internal ITextParagraphPropertiesFactoryService ParagraphPropertiesFactory;

        public FormattedLineSource(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize, TextInfoCache.FontInfo defaultFontInfo, double baseIndentation, double wordWrapWidth, double maxAutoIndent, IClassifier aggregateClassifier, ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap, ViewWrapProperties viewWrapProperties, TextInfoCache textInfoCache, IList<Lazy<ITextParagraphPropertiesFactoryService, IContentTypeMetadata>> paragraphPropertiesFactories, GuardedOperations guardedOperations, IContentTypeRegistryService contentTypeRegistry)
        {
            TopTextSnapshot = visualBufferSnapshot;
            SourceTextSnapshot = sourceTextSnapshot;
            TabSize = tabSize;
            _defaultFontInfo = defaultFontInfo;
            BaseIndentation = baseIndentation;
            WordWrapWidth = wordWrapWidth;
            MaxAutoIndent = Math.Floor(maxAutoIndent);
            AggregateClassifier = aggregateClassifier as IAccurateClassifier;
            TextAndAdornmentSequencer = sequencer;
            _textInfoCache = textInfoCache;
            _classificationFormatMap = classificationFormatMap;
            ViewWrapProperties = viewWrapProperties;
            _defaultParagraphProperties = new TextFormattingParagraphProperties(_classificationFormatMap.DefaultTextProperties, _defaultFontInfo.SpaceWidth * TabSize);
            ParagraphPropertiesFactory = SelectParagraphPropertiesFactory(sourceTextSnapshot.TextBuffer, paragraphPropertiesFactories, guardedOperations, contentTypeRegistry);
        }

        public int TabSize { get; }

        public double ColumnWidth => _defaultFontInfo.SpaceWidth;

        public double LineHeight => Math.Ceiling(_defaultFontInfo.TextHeight) + 1.0;

        public double TextHeightAboveBaseline => _defaultFontInfo.Baseline;

        public double TextHeightBelowBaseline => _defaultFontInfo.TextHeight - _defaultFontInfo.Baseline;

        public double BaseIndentation { get; }

        public double MaxAutoIndent { get; }

        public bool UseDisplayMode => _textInfoCache.UseDisplayMode;

        public TextRunProperties DefaultTextProperties => _classificationFormatMap.DefaultTextProperties;

        public double WordWrapWidth { get; }

        public ITextSnapshot TopTextSnapshot { get; }

        public ITextSnapshot SourceTextSnapshot { get; }

        public ITextAndAdornmentSequencer TextAndAdornmentSequencer { get; }

        public Collection<IFormattedLine> FormatLineInVisualBuffer(ITextSnapshotLine visualLine)
        {
            return FormatLineInVisualBuffer(visualLine, new CancellationToken?());
        }

        public Collection<IFormattedLine> FormatLineInVisualBuffer(ITextSnapshotLine visualLine, CancellationToken? cancel)
        {
            if (visualLine == null)
                throw new ArgumentNullException(nameof(visualLine));
            if (visualLine.Snapshot != TopTextSnapshot)
                throw new ArgumentException("line's Snapshot is incorrect; it must belong to the top buffer");
            if (_inFormat)
                throw new InvalidOperationException("Attempt to make a reentrant call into IFormattedLineSource");
            var normalizedSpans = NormalizedSpanGenerator.Create(TextAndAdornmentSequencer.CreateTextAndAdornmentCollection(visualLine, SourceTextSnapshot), SourceTextSnapshot, AggregateClassifier, _classificationFormatMap, cancel);
            return Format(visualLine, normalizedSpans);
        }

        public Collection<IFormattedLine> FormatLineInVisualBufferIfChanged(ITextSnapshotLine visualLine, IList<IFormattedLine> oldLines, CancellationToken? cancel)
        {
            if (visualLine == null)
                throw new ArgumentNullException(nameof(visualLine));
            if (visualLine.Snapshot != TopTextSnapshot)
                throw new ArgumentException("line's Snapshot is incorrect; it must belong to the top buffer");
            if (_inFormat)
                throw new InvalidOperationException("Attempt to make a reentrant call into IFormattedLineSource");
            var normalizedSpanList = NormalizedSpanGenerator.Create(TextAndAdornmentSequencer.CreateTextAndAdornmentCollection(visualLine, SourceTextSnapshot), SourceTextSnapshot, AggregateClassifier, _classificationFormatMap, cancel);
            if (oldLines.Count > 0)
            {
                if (oldLines[0] is FormattedLine oldLine && AreNormalizedSpansEquivalent(oldLine.NormalizedSpans, normalizedSpanList))
                    return null;
            }
            return Format(visualLine, normalizedSpanList);
        }

        internal static bool AreNormalizedSpansEquivalent(IList<NormalizedSpan> oldNormalizedSpans, IList<NormalizedSpan> newNormalizedSpans)
        {
            if (oldNormalizedSpans.Count != newNormalizedSpans.Count)
                return false;
            for (var index = 0; index < oldNormalizedSpans.Count; ++index)
            {
                var oldNormalizedSpan = oldNormalizedSpans[index];
                if (oldNormalizedSpan.Element != null)
                    return false;
                var newNormalizedSpan = newNormalizedSpans[index];
                if (newNormalizedSpan.Element != null)
                    return false;
                if (oldNormalizedSpan.ClassifiedRun != null && newNormalizedSpan.ClassifiedRun != null)
                {
                    var bufferSpan = oldNormalizedSpan.BufferSpan;
                    var length1 = bufferSpan.Length;
                    bufferSpan = newNormalizedSpan.BufferSpan;
                    var length2 = bufferSpan.Length;
                    if (length1 != length2 || oldNormalizedSpan.ClassifiedRun.Properties != newNormalizedSpan.ClassifiedRun.Properties)
                        return false;
                }
                else if (oldNormalizedSpan.ClassifiedRun != null || newNormalizedSpan.ClassifiedRun != null)
                    return false;
            }
            return true;
        }

        private Collection<IFormattedLine> Format(ITextSnapshotLine line, IList<NormalizedSpan> normalizedSpans)
        {
            try
            {
                _inFormat = true;
                var normalizedSpanManager = new NormalizedSpanManager(TopTextSnapshot, SourceTextSnapshot, TextAndAdornmentSequencer.BufferGraph, normalizedSpans, line.LineBreakLength, ViewWrapProperties, _textInfoCache, _defaultFontInfo.SpaceWidth);
                var indentation = BaseIndentation;
                var num1 = Math.Max(WordWrapWidth - indentation, 0.0);
                var maxTextWidth = ViewWrapProperties == null || num1 - ViewWrapProperties.Width <= 0.0 ? num1 : num1 - ViewWrapProperties.Width;
                var flag = MaxAutoIndent > indentation;
                var collection = new Collection<IFormattedLine>();
                var lineSegmentNumber = 0;
                while (true)
                {
                    var lineStart = MappingPointSnapshot.Create(line.Snapshot, normalizedSpanManager.CurrentSnapshotPoint, PointTrackingMode.Negative, TextAndAdornmentSequencer.BufferGraph);
                    var paragraphProperties = GetTextParagraphProperties(MappingSpanSnapshot.Create(line.Snapshot, line.Extent, SpanTrackingMode.EdgeNegative, TextAndAdornmentSequencer.BufferGraph), lineStart, lineSegmentNumber);
                    var line1 = normalizedSpanManager.FormatLine(indentation, maxTextWidth, paragraphProperties);
                    collection.Add(line1);
                    if (normalizedSpanManager.MoreLinesToFormat)
                    {
                        if (flag)
                        {
                            flag = false;
                            var endOfWhiteSpace = FindEndOfWhiteSpace(line1);
                            if (endOfWhiteSpace.HasValue)
                            {
                                indentation = Math.Min(MaxAutoIndent, Math.Floor(line1.GetCharacterBounds(endOfWhiteSpace.Value).Left + TabSize / 2 * _defaultFontInfo.SpaceWidth));
                                var num2 = WordWrapWidth - indentation;
                                maxTextWidth = ViewWrapProperties == null || num2 - ViewWrapProperties.Width <= 0.0 ? num2 : num2 - ViewWrapProperties.Width;
                            }
                        }
                        ++lineSegmentNumber;
                    }
                    else
                        break;
                }
                return collection;
            }
            finally
            {
                _inFormat = false;
            }
        }

        private ITextParagraphPropertiesFactoryService SelectParagraphPropertiesFactory(ITextBuffer textBuffer, IList<Lazy<ITextParagraphPropertiesFactoryService, IContentTypeMetadata>> paragraphPropertiesFactories, GuardedOperations guardedOperations, IContentTypeRegistryService contentTypeRegistry)
        {
            if (paragraphPropertiesFactories.Count > 0)
                return guardedOperations.InvokeBestMatchingFactory(paragraphPropertiesFactories, textBuffer.ContentType, contentTypeRegistry, this);
            return null;
        }

        private TextParagraphProperties GetTextParagraphProperties(IMappingSpan line, IMappingPoint lineStart, int lineSegmentNumber)
        {
            TextParagraphProperties paragraphProperties = null;
            if (ParagraphPropertiesFactory != null)
                paragraphProperties = ParagraphPropertiesFactory.Create(this, _classificationFormatMap.DefaultTextProperties, line, lineStart, lineSegmentNumber);
            return paragraphProperties ?? _defaultParagraphProperties;
        }

        private static SnapshotPoint? FindEndOfWhiteSpace(IFormattedLine line)
        {
            for (int start = line.Start; start < line.End; ++start)
            {
                if (!char.IsWhiteSpace(line.Snapshot[start]))
                    return new SnapshotPoint(line.Snapshot, start);
            }
            return new SnapshotPoint?();
        }
    }
}