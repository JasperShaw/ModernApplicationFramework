using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.TextEditor.Utilities;
using TextBounds = ModernApplicationFramework.Text.Ui.Formatting.TextBounds;

namespace ModernApplicationFramework.TextEditor.Text.Formatting
{
    internal sealed class FormattedLine : IFormattedLine
    {
        private TextViewLineChange _change = TextViewLineChange.NewOrReformatted;
        private readonly Span _tokenIndexSpan;
        private SnapshotSpan _extentIncludingLineBreak;
        private readonly IBufferGraph _bufferGraph;
        private readonly int _lineBreakLength;
        private IList<TextLineData> _textLines;
        private ITextSnapshot _visualSnapshot;
        private readonly BufferPositionToTokenIndexMap _bufferToTokenMap;
        private readonly Dictionary<object, int> _adornmentToTokenMap;
        private readonly Dictionary<object, FrugalList<object>> _providerToAdornmentMap;
        private double _deltaY;
        private VisibilityState _visibilityState;
        private RenderedLineVisual _visual;
        private LineTransform _transform;
        private Rect _visibleArea;
        private readonly double _width;
        private readonly double _textWidth;
        private double _height;
        private double _topSpace;
        private double _textHeight;
        private double _top;
        private readonly double _virtualSpaceWidth;
        private readonly double _textLeft;
        private readonly double _textRight;
        private readonly double _unscaledTopSpace;
        private readonly double _unscaledTextHeight;
        private readonly double _unscaledBottomSpace;
        private readonly bool _mayContainMultiCharacterGlyphs;

        internal ViewWrapProperties ViewWrapProperties { get; }

        public FormattedLine(double indentation, IList<TextLineData> textLines, IBufferGraph bufferGraph, ITextSnapshot visualSnapshot, NormalizedSpanTextSource textSource, BufferPositionToTokenIndexMap bufferToTokenMap, int startTokenIndex, int endTokenIndex, int lineBreakLength, bool isFirstTextViewLineForSnapshotLine, bool isLastTextViewLineForSnapshotLine, ViewWrapProperties viewWrapProperties, TextInfoCache textInfoCache, double defaultSpaceWidth)
        {
            _bufferGraph = bufferGraph;
            _visualSnapshot = visualSnapshot;
            NormalizedSpans = textSource.NormalizedSpans;
            _textLeft = indentation;
            _textLines = textLines;
            _bufferToTokenMap = bufferToTokenMap;
            _lineBreakLength = lineBreakLength;
            IsFirstTextViewLineForSnapshotLine = isFirstTextViewLineForSnapshotLine;
            IsLastTextViewLineForSnapshotLine = isLastTextViewLineForSnapshotLine;
            _tokenIndexSpan = Span.FromBounds(startTokenIndex, endTokenIndex);
            ResetExtent();
            var val1 = 0.0;
            _virtualSpaceWidth = defaultSpaceWidth;
            var tokenIndex = startTokenIndex;
            do
            {
                var normalizedSpan = textSource.FindNormalizedSpan(tokenIndex);
                Span span;
                if (normalizedSpan.ClassifiedRun != null)
                {
                    var textInfo = textInfoCache.GetTextInfo(normalizedSpan.ClassifiedRun.Properties);
                    UnscaledBaseline = Math.Max(UnscaledBaseline, textInfo.Baseline);
                    val1 = Math.Max(val1, textInfo.TextHeight - textInfo.Baseline);
                    _mayContainMultiCharacterGlyphs = _mayContainMultiCharacterGlyphs || normalizedSpan.ClassifiedRun.MayContainMultiCharacterGlyphs;
                    span = normalizedSpan.BufferSpan;
                    if (span.Start < _extentIncludingLineBreak.End.Position - lineBreakLength)
                        _virtualSpaceWidth = textInfo.SpaceWidth;
                }
                else if (normalizedSpan.Element != null)
                {
                    _unscaledTopSpace = Math.Max(_unscaledTopSpace, normalizedSpan.Element.TopSpace);
                    UnscaledBaseline = Math.Max(UnscaledBaseline, normalizedSpan.Element.Baseline);
                    val1 = Math.Max(val1, normalizedSpan.Element.TextHeight - normalizedSpan.Element.Baseline);
                    _unscaledBottomSpace = Math.Max(_unscaledBottomSpace, normalizedSpan.Element.BottomSpace);
                    if (_adornmentToTokenMap == null)
                        _adornmentToTokenMap = new Dictionary<object, int>();
                    try
                    {
                        var adornmentToTokenMap = _adornmentToTokenMap;
                        var identityTag = normalizedSpan.Element.IdentityTag;
                        span = normalizedSpan.TokenSpan;
                        var start = span.Start;
                        adornmentToTokenMap.Add(identityTag, start);
                    }
                    catch (ArgumentException)
                    {
                    }
                    if (normalizedSpan.Element.ProviderTag != null)
                    {
                        if (_providerToAdornmentMap == null)
                            _providerToAdornmentMap = new Dictionary<object, FrugalList<object>>();
                        if (!_providerToAdornmentMap.TryGetValue(normalizedSpan.Element.ProviderTag, out var frugalList))
                        {
                            frugalList = new FrugalList<object>();
                            _providerToAdornmentMap.Add(normalizedSpan.Element.ProviderTag, frugalList);
                        }
                        frugalList.Add(normalizedSpan.Element.IdentityTag);
                    }
                }
                span = normalizedSpan.TokenSpan;
                tokenIndex = span.End;
            }
            while (tokenIndex < endTokenIndex);
            _unscaledTextHeight = Math.Ceiling(UnscaledBaseline + val1);
            SetTop(0.0);
            _textWidth = _textLines[_textLines.Count - 1].Right - _textLines[0].Left;
            _width = _lineBreakLength > 0 ? _textWidth + EndOfLineWidth : _textWidth;
            _textRight = _textLeft + _textWidth;
            SetLineTransform(DefaultLineTransform);
            ViewWrapProperties = viewWrapProperties;
        }

        public IList<NormalizedSpan> NormalizedSpans { get; }

        private void ThrowIfInvalid()
        {
            if (!IsValid)
                throw new ObjectDisposedException(nameof(FormattedLine));
        }

        private SnapshotPoint FixBufferPosition(SnapshotPoint bufferPosition)
        {
            if (bufferPosition.Snapshot != _extentIncludingLineBreak.Snapshot)
                throw new ArgumentException();
            return bufferPosition;
        }

        private VirtualSnapshotPoint FixVirtualBufferPosition(VirtualSnapshotPoint virtualBufferPosition)
        {
            if (virtualBufferPosition.Position.Snapshot != _extentIncludingLineBreak.Snapshot)
                throw new ArgumentException();
            return virtualBufferPosition;
        }

        private SnapshotSpan FixBufferSpan(SnapshotSpan bufferSpan)
        {
            if (bufferSpan.Snapshot != _extentIncludingLineBreak.Snapshot)
                throw new ArgumentException();
            return bufferSpan;
        }

        private TextLineData GetTextLineContainingTokenIndex(int tokenIndex)
        {
            foreach (var textLine in _textLines)
            {
                if (tokenIndex < textLine.TokenSpan.End)
                    return textLine;
            }

            return _textLines[_textLines.Count - 1];
        }

        private TextBounds GetTokenBounds(int tokenIndex)
        {
            var containingTokenIndex = GetTextLineContainingTokenIndex(tokenIndex);
            var tokenSpan = containingTokenIndex.TokenSpan;
            var start = tokenSpan.Start;
            tokenSpan = containingTokenIndex.TokenSpan;
            var val2 = Math.Min(tokenSpan.End, tokenIndex);
            tokenIndex = Math.Max(start, val2);
            var fromCharacterHit1 = containingTokenIndex.TextLine.GetDistanceFromCharacterHit(new CharacterHit(tokenIndex, 0));
            var fromCharacterHit2 = containingTokenIndex.TextLine.GetDistanceFromCharacterHit(new CharacterHit(tokenIndex, 1));
            return new TextBounds(containingTokenIndex.Left + fromCharacterHit1, Top, fromCharacterHit2 - fromCharacterHit1, Height, TextTop, TextHeight);
        }

        private void ResetExtent()
        {
            _extentIncludingLineBreak = new SnapshotSpan(_bufferToTokenMap.SourceTextSnapshot, Span.FromBounds(_bufferToTokenMap.GetNextBufferPositionFromTokenIndex(_tokenIndexSpan.Start), _bufferToTokenMap.GetNextBufferPositionFromTokenIndex(_tokenIndexSpan.End)));
        }

        private bool RelaxedContainsBufferPosition(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            bufferPosition = FixBufferPosition(bufferPosition);
            if (bufferPosition < _extentIncludingLineBreak.Start)
                return false;
            if (bufferPosition < _extentIncludingLineBreak.End)
                return true;
            if (bufferPosition == _extentIncludingLineBreak.End)
                return _lineBreakLength == 0;
            return false;
        }

        public Visual GetOrCreateVisual()
        {
            ThrowIfInvalid();
            return _visual ?? (_visual = new RenderedLineVisual(this));
        }

        public void RemoveVisual()
        {
            _visual = null;
        }

        public void SetChange(TextViewLineChange change)
        {
            _change = change;
        }

        public void SetDeltaY(double deltaY)
        {
            ThrowIfInvalid();
            _deltaY = deltaY;
        }

        public void SetLineTransform(LineTransform transform)
        {
            ThrowIfInvalid();
            _transform = transform;
            _topSpace = Math.Ceiling(Math.Max(_transform.TopSpace, _unscaledTopSpace));
            _textHeight = Math.Ceiling(_unscaledTextHeight * _transform.VerticalScale);
            _height = _textHeight + _topSpace + Math.Ceiling(Math.Max(_transform.BottomSpace, _unscaledBottomSpace));
            _visual?.SetTransform();
        }

        public void SetSnapshot(ITextSnapshot visualSnapshot, ITextSnapshot editSnapshot)
        {
            ThrowIfInvalid();
            _visualSnapshot = visualSnapshot;
            _bufferToTokenMap.SetSourceTextSnapshot(editSnapshot);
            ResetExtent();
        }

        public void SetTop(double top)
        {
            ThrowIfInvalid();
            _top = top;
            _visual?.SetTransform();
        }

        public void SetVisibleArea(Rect visibleArea)
        {
            ThrowIfInvalid();
            _visibleArea = visibleArea;
            if (Bottom < visibleArea.Top + 0.01 || Top > visibleArea.Bottom - 0.01)
            {
                _visibilityState = VisibilityState.Hidden;
                _visual = null;
            }
            else
            {
                if (_visual != null && (_visibleArea.Left < _visual.RenderedLeftEdge || _visibleArea.Right > _visual.RenderedRightEdge))
                    _visual.RenderText();
                if (Top >= visibleArea.Top - 0.01 && Bottom <= visibleArea.Bottom + 0.01)
                    _visibilityState = VisibilityState.FullyVisible;
                else
                    _visibilityState = VisibilityState.PartiallyVisible;
            }
        }

        public Rect VisibleArea
        {
            get
            {
                ThrowIfInvalid();
                return _visibleArea;
            }
        }

        public TextRunProperties GetCharacterFormatting(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            bufferPosition = FixBufferPosition(bufferPosition);
            if (!ContainsBufferPosition(bufferPosition))
                throw new ArgumentOutOfRangeException(nameof(bufferPosition));
            if (bufferPosition >= _extentIncludingLineBreak.End)
                return null;
            var fromBufferPosition = _bufferToTokenMap.GetTokenIndexFromBufferPosition(bufferPosition);
            var containingTokenIndex = GetTextLineContainingTokenIndex(fromBufferPosition);
            var num = fromBufferPosition - containingTokenIndex.TokenSpan.Start;
            var textRunSpans = containingTokenIndex.TextLine.GetTextRunSpans();
            foreach (var textSpan in textRunSpans)
            {
                if (textSpan.Length > num)
                    return textSpan.Value.Properties;
                num -= textSpan.Length;
            }
            return null;
        }

        public ReadOnlyCollection<TextLine> TextLines
        {
            get
            {
                ThrowIfInvalid();
                var frugalList = new FrugalList<TextLine>();
                for (var index = 0; index < _textLines.Count; ++index)
                    frugalList.Add(_textLines[index].TextLine);
                return new ReadOnlyCollection<TextLine>(frugalList);
            }
        }

        public SnapshotPoint? GetBufferPositionFromXCoordinate(double xCoordinate)
        {
            return GetBufferPositionFromXCoordinate(xCoordinate, false);
        }

        public SnapshotPoint? GetBufferPositionFromXCoordinate(double xCoordinate, bool textOnly)
        {
            ThrowIfInvalid();
            if (double.IsNaN(xCoordinate))
                throw new ArgumentOutOfRangeException(nameof(xCoordinate));
            if (xCoordinate < _textLeft || xCoordinate >= _width + _textLeft)
                return new SnapshotPoint?();
            if (xCoordinate >= _textRight)
                return _extentIncludingLineBreak.End - _lineBreakLength;
            foreach (var textLine in _textLines)
            {
                if (xCoordinate < textLine.Right)
                {
                    var firstCharacterIndex = textLine.TextLine.GetCharacterHitFromDistance(xCoordinate - textLine.Left).FirstCharacterIndex;
                    if (textOnly && !_bufferToTokenMap.IsTokenIndexABufferPosition(firstCharacterIndex))
                        return new SnapshotPoint?();
                    return new SnapshotPoint(_bufferToTokenMap.SourceTextSnapshot, Math.Max(Start.Position, Math.Min(End.Position, _bufferToTokenMap.GetAssociatedBufferPositionFromTokenIndex(firstCharacterIndex))));
                }
            }
            return new SnapshotPoint?();
        }

        public VirtualSnapshotPoint GetVirtualBufferPositionFromXCoordinate(double x)
        {
            ThrowIfInvalid();
            var virtualSpaces = 0;
            var nullable = GetBufferPositionFromXCoordinate(x);
            if (!nullable.HasValue)
            {
                if (x <= _textLeft)
                {
                    nullable = _extentIncludingLineBreak.Start;
                }
                else
                {
                    nullable = _extentIncludingLineBreak.End - _lineBreakLength;
                    if (IsLastTextViewLineForSnapshotLine && x > _textRight)
                        virtualSpaces = (int)((x - _textRight) / _virtualSpaceWidth);
                }
            }
            return new VirtualSnapshotPoint(nullable.Value, virtualSpaces);
        }

        public VirtualSnapshotPoint GetInsertionBufferPositionFromXCoordinate(double x)
        {
            ThrowIfInvalid();
            var virtualSpaces = 0;
            var nullable = GetBufferPositionFromXCoordinate(x);
            if (nullable.HasValue)
            {
                var end = nullable.Value;
                var position = end.Position;
                end = _extentIncludingLineBreak.End;
                var num = end.Position - _lineBreakLength;
                if (position < num)
                {
                    var extendedCharacterBounds = GetExtendedCharacterBounds(nullable.Value);
                    if (extendedCharacterBounds.IsRightToLeft == x < extendedCharacterBounds.Left + extendedCharacterBounds.Width * 0.5)
                    {
                        nullable = GetTextElementSpan(nullable.Value).End;
                    }
                    return new VirtualSnapshotPoint(nullable.Value, virtualSpaces);
                }
            }
            if (x <= _textLeft)
            {
                nullable = _extentIncludingLineBreak.Start;
            }
            else
            {
                nullable = _extentIncludingLineBreak.End - _lineBreakLength;
                if (IsLastTextViewLineForSnapshotLine && x > _textRight)
                    virtualSpaces = (int)Math.Round((x - _textRight) / _virtualSpaceWidth);
            }
            return new VirtualSnapshotPoint(nullable.Value, virtualSpaces);
        }

        public bool ContainsBufferPosition(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            bufferPosition = FixBufferPosition(bufferPosition);
            if (bufferPosition < _extentIncludingLineBreak.Start)
                return false;
            if (bufferPosition < _extentIncludingLineBreak.End)
                return true;
            if (bufferPosition == _extentIncludingLineBreak.End && _lineBreakLength == 0)
                return IsLastTextViewLineForSnapshotLine;
            return false;
        }

        public SnapshotSpan GetTextElementSpan(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            bufferPosition = FixBufferPosition(bufferPosition);
            if (!ContainsBufferPosition(bufferPosition))
                throw new ArgumentOutOfRangeException(nameof(bufferPosition));
            if (bufferPosition >= _extentIncludingLineBreak.End - _lineBreakLength)
                return new SnapshotSpan(_extentIncludingLineBreak.End - _lineBreakLength, _lineBreakLength);
            var elisionSpan = _bufferToTokenMap.GetElisionSpan(bufferPosition);
            if (elisionSpan.HasValue)
                return new SnapshotSpan(_bufferToTokenMap.SourceTextSnapshot, elisionSpan.Value);
            if (!_mayContainMultiCharacterGlyphs)
                return new SnapshotSpan(bufferPosition, 1);
            var fromBufferPosition = _bufferToTokenMap.GetTokenIndexFromBufferPosition(bufferPosition);
            var textLine = GetTextLineContainingTokenIndex(fromBufferPosition).TextLine;
            var caretCharacterHit = textLine.GetNextCaretCharacterHit(new CharacterHit(fromBufferPosition, 0));
            if (caretCharacterHit.FirstCharacterIndex > fromBufferPosition)
                return new SnapshotSpan(bufferPosition, 1);
            var firstCharacterIndex1 = caretCharacterHit.FirstCharacterIndex + caretCharacterHit.TrailingLength;
            caretCharacterHit = textLine.GetPreviousCaretCharacterHit(new CharacterHit(firstCharacterIndex1, 0));
            var firstCharacterIndex2 = caretCharacterHit.FirstCharacterIndex;
            return new SnapshotSpan(_bufferToTokenMap.SourceTextSnapshot, _bufferToTokenMap.GetNextBufferPositionFromTokenIndex(firstCharacterIndex2), firstCharacterIndex1 - firstCharacterIndex2);
        }

        public TextBounds GetCharacterBounds(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            bufferPosition = FixBufferPosition(bufferPosition);
            if (!RelaxedContainsBufferPosition(bufferPosition))
                throw new ArgumentOutOfRangeException(nameof(bufferPosition));
            return bufferPosition >= (_extentIncludingLineBreak.End - _lineBreakLength)
                ? new TextBounds(_textRight, Top, EndOfLineWidth, Height, TextTop, TextHeight)
                : GetTokenBounds(
                    _bufferToTokenMap.GetTokenIndexFromBufferPosition(GetTextElementSpan(bufferPosition).Start));
        }

        public TextBounds GetCharacterBounds(VirtualSnapshotPoint virtualBufferPosition)
        {
            ThrowIfInvalid();
            virtualBufferPosition = FixVirtualBufferPosition(virtualBufferPosition);
            if (!virtualBufferPosition.IsInVirtualSpace)
                return GetCharacterBounds(virtualBufferPosition.Position);
            if (IsLastTextViewLineForSnapshotLine)
                return new TextBounds(_textRight + virtualBufferPosition.VirtualSpaces * _virtualSpaceWidth, _top, _virtualSpaceWidth, _height, TextTop, TextHeight);
            return new TextBounds(_textRight, _top, EndOfLineWidth, _height, TextTop, TextHeight);
        }

        public TextBounds GetExtendedCharacterBounds(VirtualSnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            if (bufferPosition.IsInVirtualSpace)
                return GetCharacterBounds(bufferPosition);
            return GetExtendedCharacterBounds(bufferPosition.Position);
        }

        public TextBounds GetExtendedCharacterBounds(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            bufferPosition = FixBufferPosition(bufferPosition);
            if (!RelaxedContainsBufferPosition(bufferPosition))
                throw new ArgumentOutOfRangeException(nameof(bufferPosition));
            if (bufferPosition >= (_extentIncludingLineBreak.End - _lineBreakLength))
            {
                var leading = !IsLastTextViewLineForSnapshotLine ? _textRight : GetTokenBounds(_bufferToTokenMap.GetLeftmostTokenIndexForBufferPosition(_extentIncludingLineBreak.End.Position - _lineBreakLength)).Left;
                var num = _textRight + EndOfLineWidth;
                return new TextBounds(leading, Top, num - leading, Height, TextTop, TextHeight);
            }
            var textElementSpan = GetTextElementSpan(bufferPosition);
            var fromBufferPosition = _bufferToTokenMap.GetTokenIndexFromBufferPosition(textElementSpan.Start);
            var num1 = !_bufferToTokenMap.GetElisionSpan(textElementSpan.Start).HasValue ? fromBufferPosition + textElementSpan.Length - 1 : fromBufferPosition;
            var tokenBounds = GetTokenBounds(fromBufferPosition);
            var forBufferPosition1 = _bufferToTokenMap.GetLeftmostTokenIndexForBufferPosition(textElementSpan.Start);
            var forBufferPosition2 = _bufferToTokenMap.GetRightmostTokenIndexForBufferPosition(textElementSpan.End.Position - 1);
            if (fromBufferPosition == forBufferPosition1 && num1 == forBufferPosition2)
                return tokenBounds;
            var leading1 = GetTokenBounds(forBufferPosition1).Left;
            var num2 = GetTokenBounds(forBufferPosition2).Right;
            if (tokenBounds.IsRightToLeft)
            {
                var num3 = leading1;
                leading1 = num2;
                num2 = num3;
            }
            return new TextBounds(leading1, tokenBounds.Top, num2 - leading1, tokenBounds.Height, tokenBounds.TextTop, tokenBounds.TextHeight);
        }

        public TextBounds? GetAdornmentBounds(object identityTag)
        {
            ThrowIfInvalid();
            if (_adornmentToTokenMap != null && _adornmentToTokenMap.TryGetValue(identityTag, out var tokenIndex))
                return GetTokenBounds(tokenIndex);
            return new TextBounds?();
        }

        public Collection<TextBounds> GetNormalizedTextBounds(SnapshotSpan bufferSpan)
        {
            ThrowIfInvalid();
            bufferSpan = FixBufferSpan(bufferSpan);
            var nullable = bufferSpan.Overlap(_extentIncludingLineBreak);
            IList<TextBounds> list;
            if (nullable.HasValue)
            {
                var start = GetTextElementSpan(nullable.Value.Start).Start;
                var end = nullable.Value.End;
                SnapshotPoint endElementPosition;
                if (end < _extentIncludingLineBreak.End - _lineBreakLength)
                {
                    var textElementSpan = GetTextElementSpan(end);
                    endElementPosition = end == textElementSpan.Start ? textElementSpan.Start : textElementSpan.End;
                }
                else
                    endElementPosition = _extentIncludingLineBreak.End;
                var unnormalizedBounds = GetUnnormalizedBounds(start, endElementPosition);
                if (nullable.Value.End > _extentIncludingLineBreak.End - _lineBreakLength)
                    unnormalizedBounds.Add(new Tuple<double, double>(_textRight, _textLeft + _width));
                list = new List<TextBounds>(unnormalizedBounds.Count);
                if (unnormalizedBounds.Count > 0)
                {
                    unnormalizedBounds.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                    var leading = unnormalizedBounds[0].Item1;
                    var val1 = unnormalizedBounds[0].Item2;
                    for (var index = 1; index < unnormalizedBounds.Count; ++index)
                    {
                        if (unnormalizedBounds[index].Item1 > val1 + 1.01)
                        {
                            list.Add(new TextBounds(leading, Top, val1 - leading, Height, TextTop, TextHeight));
                            leading = unnormalizedBounds[index].Item1;
                        }
                        val1 = Math.Max(val1, unnormalizedBounds[index].Item2);
                    }
                    list.Add(new TextBounds(leading, Top, val1 - leading, Height, TextTop, TextHeight));
                }
            }
            else
                list = new List<TextBounds>(0);
            return new Collection<TextBounds>(list);
        }

        private List<Tuple<double, double>> GetUnnormalizedBounds(SnapshotPoint startElementPosition, SnapshotPoint endElementPosition)
        {
            var tupleList1 = new List<Tuple<double, double>>();
            var forBufferPosition = _bufferToTokenMap.GetLeftmostTokenIndexForBufferPosition(startElementPosition);
            var num1 = _bufferToTokenMap.GetRightmostTokenIndexForBufferPosition((endElementPosition - 1)) + 1;
            var containingTokenIndex1 = GetTextLineContainingTokenIndex(forBufferPosition);
            var containingTokenIndex2 = GetTextLineContainingTokenIndex(num1);
            for (var textLineIndex = containingTokenIndex1.TextLineIndex; textLineIndex <= containingTokenIndex2.TextLineIndex; ++textLineIndex)
            {
                var textLine = _textLines[textLineIndex];
                var tokenSpan = textLine.TokenSpan;
                var firstTextSourceCharacterIndex = Math.Max(tokenSpan.Start, forBufferPosition);
                tokenSpan = textLine.TokenSpan;
                var num2 = Math.Min(tokenSpan.End, num1);
                if (firstTextSourceCharacterIndex < num2)
                {
                    if (firstTextSourceCharacterIndex == textLine.TokenSpan.Start && num2 == textLine.TokenSpan.End)
                    {
                        tupleList1.Add(new Tuple<double, double>(textLine.Left, textLine.Right));
                    }
                    else
                    {
                        foreach (var textBound in textLine.TextLine.GetTextBounds(firstTextSourceCharacterIndex, num2 - firstTextSourceCharacterIndex))
                        {
                            var tupleList2 = tupleList1;
                            var rectangle = textBound.Rectangle;
                            var num3 = rectangle.Left + textLine.Left;
                            rectangle = textBound.Rectangle;
                            var num4 = rectangle.Right + textLine.Left;
                            var tuple = new Tuple<double, double>(num3, num4);
                            tupleList2.Add(tuple);
                        }
                    }
                }
            }
            return tupleList1;
        }

        public object IdentityTag => this;

        public bool IntersectsBufferSpan(SnapshotSpan bufferSpan)
        {
            ThrowIfInvalid();
            bufferSpan = FixBufferSpan(bufferSpan);
            if ((int)bufferSpan.End < _extentIncludingLineBreak.Start)
                return false;
            if (bufferSpan.Start < _extentIncludingLineBreak.End)
                return true;
            if (bufferSpan.Start == _extentIncludingLineBreak.End)
                return _lineBreakLength == 0 & bufferSpan.Start == _extentIncludingLineBreak.Snapshot.Length;
            return false;
        }

        public ReadOnlyCollection<object> GetAdornmentTags(object providerTag)
        {
            ThrowIfInvalid();
            if (providerTag == null)
                throw new ArgumentNullException(nameof(providerTag));
            if (_providerToAdornmentMap == null || !_providerToAdornmentMap.TryGetValue(providerTag, out var frugalList))
                return new ReadOnlyCollection<object>(new FrugalList<object>());
            return new ReadOnlyCollection<object>(frugalList);
        }

        public ITextSnapshot Snapshot => _extentIncludingLineBreak.Snapshot;

        public bool IsFirstTextViewLineForSnapshotLine { get; }

        public bool IsLastTextViewLineForSnapshotLine { get; }

        public double Baseline
        {
            get
            {
                ThrowIfInvalid();
                return UnscaledBaseline * _transform.VerticalScale;
            }
        }

        public SnapshotSpan Extent
        {
            get
            {
                ThrowIfInvalid();
                return new SnapshotSpan(_extentIncludingLineBreak.Snapshot, _extentIncludingLineBreak.Start, _extentIncludingLineBreak.Length - _lineBreakLength);
            }
        }

        public IMappingSpan ExtentAsMappingSpan => MappingSpanSnapshot.Create(_visualSnapshot, new SnapshotSpan(MappingPointSnapshot.MapUpToSnapshotNoTrack(_visualSnapshot, _extentIncludingLineBreak.Start, PositionAffinity.Predecessor).Value, MappingPointSnapshot.MapUpToSnapshotNoTrack(_visualSnapshot, _extentIncludingLineBreak.End - _lineBreakLength, PositionAffinity.Successor).Value), SpanTrackingMode.EdgeInclusive, _bufferGraph);

        public SnapshotSpan ExtentIncludingLineBreak
        {
            get
            {
                ThrowIfInvalid();
                return _extentIncludingLineBreak;
            }
        }

        public IMappingSpan ExtentIncludingLineBreakAsMappingSpan => MappingSpanSnapshot.Create(_visualSnapshot, new SnapshotSpan(MappingPointSnapshot.MapUpToSnapshotNoTrack(_visualSnapshot, _extentIncludingLineBreak.Start, PositionAffinity.Predecessor).Value, MappingPointSnapshot.MapUpToSnapshotNoTrack(_visualSnapshot, _extentIncludingLineBreak.End, PositionAffinity.Successor).Value), SpanTrackingMode.EdgeInclusive, _bufferGraph);

        public SnapshotPoint Start
        {
            get
            {
                ThrowIfInvalid();
                return _extentIncludingLineBreak.Start;
            }
        }

        public int Length
        {
            get
            {
                ThrowIfInvalid();
                return _extentIncludingLineBreak.Length - _lineBreakLength;
            }
        }

        public int LengthIncludingLineBreak
        {
            get
            {
                ThrowIfInvalid();
                return _extentIncludingLineBreak.Length;
            }
        }

        public SnapshotPoint End
        {
            get
            {
                ThrowIfInvalid();
                return _extentIncludingLineBreak.End - _lineBreakLength;
            }
        }

        public SnapshotPoint EndIncludingLineBreak
        {
            get
            {
                ThrowIfInvalid();
                return _extentIncludingLineBreak.End;
            }
        }

        public int LineBreakLength
        {
            get
            {
                ThrowIfInvalid();
                return _lineBreakLength;
            }
        }

        public double Left
        {
            get
            {
                ThrowIfInvalid();
                return _textLeft;
            }
        }

        public double Top
        {
            get
            {
                ThrowIfInvalid();
                return _top;
            }
        }

        public double Height
        {
            get
            {
                ThrowIfInvalid();
                return _height;
            }
        }

        public double TextTop
        {
            get
            {
                ThrowIfInvalid();
                return _top + _topSpace;
            }
        }

        public double TextBottom
        {
            get
            {
                ThrowIfInvalid();
                return _top + _topSpace + _textHeight;
            }
        }

        public double TextHeight
        {
            get
            {
                ThrowIfInvalid();
                return _textHeight;
            }
        }

        public double TextLeft
        {
            get
            {
                ThrowIfInvalid();
                return _textLeft;
            }
        }

        public double TextRight
        {
            get
            {
                ThrowIfInvalid();
                return _textRight;
            }
        }

        public double TextWidth
        {
            get
            {
                ThrowIfInvalid();
                return _textWidth;
            }
        }

        public double Width
        {
            get
            {
                ThrowIfInvalid();
                return _width;
            }
        }

        public double Bottom
        {
            get
            {
                ThrowIfInvalid();
                return _top + _height;
            }
        }

        public double Right
        {
            get
            {
                ThrowIfInvalid();
                return _textLeft + _width;
            }
        }

        public double EndOfLineWidth
        {
            get
            {
                ThrowIfInvalid();
                return Math.Floor(_unscaledTextHeight * (7.0 / 12.0));
            }
        }

        public double VirtualSpaceWidth
        {
            get
            {
                ThrowIfInvalid();
                return _virtualSpaceWidth;
            }
        }

        public bool IsValid => _textLines != null;

        public LineTransform LineTransform
        {
            get
            {
                ThrowIfInvalid();
                return _transform;
            }
        }

        public LineTransform DefaultLineTransform
        {
            get
            {
                ThrowIfInvalid();
                return new LineTransform(0.0, 1.0, 1.0, Right);
            }
        }

        public VisibilityState VisibilityState
        {
            get
            {
                ThrowIfInvalid();
                return _visibilityState;
            }
        }

        public double DeltaY
        {
            get
            {
                ThrowIfInvalid();
                return _deltaY;
            }
        }

        public TextViewLineChange Change
        {
            get
            {
                ThrowIfInvalid();
                return _change;
            }
        }

        public void Dispose()
        {
            if (!IsValid)
                return;
            for (var index = 0; index < _textLines.Count; ++index)
                _textLines[index].TextLine.Dispose();
            _textLines = null;
        }

        public double UnscaledBaseline { get; }

        internal class RenderedLineVisual : DrawingVisual
        {
            private readonly FormattedLine _textViewLine;
            private MatrixTransform _transform;

            public RenderedLineVisual(FormattedLine textViewLine)
            {
                _textViewLine = textViewLine;
                SetTransform();
                RenderText();
            }

            public double RenderedLeftEdge { get; private set; }

            public double RenderedRightEdge { get; private set; }

            public void SetTransform()
            {
                _transform = new MatrixTransform(1.0, 0.0, 0.0, _textViewLine.LineTransform.VerticalScale, 0.0, _textViewLine.TextTop);
                _transform.Freeze();
                Transform = _transform;
            }

            public void RenderText()
            {
                var drawingContext = RenderOpen();
                RenderedLeftEdge = double.MinValue;
                RenderedRightEdge = double.MaxValue;
                var unscaledBaseline = _textViewLine.UnscaledBaseline;
                var x = _textViewLine.Left;
                var textLines = _textViewLine.TextLines;
                foreach (var textLine in textLines)
                {
                    var num1 = x;
                    var visibleArea = _textViewLine.VisibleArea;
                    var right = visibleArea.Right;
                    if (num1 > right)
                    {
                        RenderedRightEdge = x;
                        break;
                    }
                    var num2 = x + textLine.WidthIncludingTrailingWhitespace;
                    var num3 = num2;
                    visibleArea = _textViewLine.VisibleArea;
                    var left = visibleArea.Left;
                    if (num3 >= left)
                    {
                        var num4 = unscaledBaseline - textLine.Baseline;
                        textLine.Draw(drawingContext, new Point(x, num4), InvertAxes.None);
                        if (_textViewLine.ViewWrapProperties != null)
                            DrawViewWrapIndicator(drawingContext, _textViewLine, num4);
                    }
                    else
                        RenderedLeftEdge = num2;
                    x = num2;
                }
                drawingContext.Close();
            }

            private static void DrawViewWrapIndicator(DrawingContext drawingContext, FormattedLine line, double lineTop)
            {
                var textHeight = line.TextHeight;
                var width = line.ViewWrapProperties.Width;
                var num1 = Math.Max(line.Right, line.VisibleArea.Right - width);
                var num2 = lineTop;
                var foregroundBrush = line.ViewWrapProperties.ForegroundBrush;
                if (foregroundBrush.CanFreeze)
                    foregroundBrush.Freeze();
                var thickness = 0.08 * textHeight;
                var arcSegment = new ArcSegment(new Point(num1 + width / 2.0, num2 + textHeight / 2.0 + textHeight / 8.0), new Size(width / 2.0, textHeight / 8.0), 180.0, false, SweepDirection.Clockwise, true);
                arcSegment.Freeze();
                var pathFigure1 = new PathFigure(new Point(num1 + width / 8.0, num2 + textHeight / 2.0 - textHeight / 8.0), new PathSegment[]
                {
                    arcSegment
                }, false);
                pathFigure1.Freeze();
                var pen1 = new Pen(foregroundBrush, thickness);
                pen1.Freeze();
                var pathGeometry1 = new PathGeometry(new[]
                {
                    pathFigure1
                });
                pathGeometry1.Freeze();
                drawingContext.DrawGeometry(null, pen1, pathGeometry1);
                var polyLineSegment = new PolyLineSegment();
                polyLineSegment.Points.Add(new Point(num1 + width / 2.0, num2 + textHeight / 2.0 + textHeight / 4.0));
                polyLineSegment.Points.Add(new Point(num1 + width / 2.0, num2 + textHeight / 2.0));
                polyLineSegment.Freeze();
                var pathFigure2 = new PathFigure(new Point(num1 + width / 2.0 - textHeight / 8.0, num2 + textHeight / 2.0 + textHeight / 8.0), new PathSegment[]
                {
                    polyLineSegment
                }, true);
                pathFigure2.Freeze();
                var pen2 = new Pen(foregroundBrush, thickness / 2.0);
                pen2.Freeze();
                var pathGeometry2 = new PathGeometry(new[]
                {
                    pathFigure2
                });
                pathGeometry2.Freeze();
                drawingContext.DrawGeometry(foregroundBrush, pen2, pathGeometry2);
            }
        }
    }
}