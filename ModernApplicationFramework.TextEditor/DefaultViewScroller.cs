using System;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class DefaultViewScroller : IViewScroller
    {
        private TextView _textView;
        private TextEditorFactoryService _factoryService;

        public DefaultViewScroller(TextView textView, TextEditorFactoryService factoryService)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _factoryService = factoryService ?? throw new ArgumentNullException(nameof(factoryService));
        }

        public void ScrollViewportVerticallyByPixels(double pixelsToScroll)
        {
            if (double.IsNaN(pixelsToScroll))
                throw new ArgumentOutOfRangeException(nameof(pixelsToScroll));
            var textViewLines = _textView.TextViewLines;
            if (textViewLines == null)
                return;
            if (pixelsToScroll >= 0.0)
            {
                var textViewLine = textViewLines[0];
                _textView.DisplayTextLineContainingBufferPosition(textViewLine.Start, textViewLine.Top - _textView.ViewportTop + pixelsToScroll, ViewRelativePosition.Top);
            }
            else
            {
                var textViewLine = textViewLines[textViewLines.Count - 1];
                _textView.DisplayTextLineContainingBufferPosition(textViewLine.Start, textViewLine.Top - _textView.ViewportTop + pixelsToScroll, ViewRelativePosition.Top);
            }
        }

        public void ScrollViewportVerticallyByLine(ScrollDirection direction)
        {
            switch (direction)
            {
                case ScrollDirection.Up:
                case ScrollDirection.Down:
                    var textViewLines = _textView.TextViewLines;
                    if (textViewLines == null)
                        break;
                    var val1_1 = 0.0;
                    var firstVisibleLine = _textView.TextViewLines.FirstVisibleLine;
                    double pixelsToScroll;
                    if (direction == ScrollDirection.Up)
                    {
                        if (firstVisibleLine.Start > 0)
                        {
                            var containingBufferPosition = textViewLines.GetTextViewLineContainingBufferPosition(firstVisibleLine.Start - 1);
                            if (containingBufferPosition != null)
                                val1_1 = containingBufferPosition.Height;
                        }
                        if (firstVisibleLine.VisibilityState != VisibilityState.FullyVisible)
                            val1_1 += _textView.ViewportTop - firstVisibleLine.Top;
                        pixelsToScroll = Math.Min(val1_1, _textView.ViewportHeight);
                    }
                    else
                    {
                        double val1_2;
                        if (firstVisibleLine.VisibilityState == VisibilityState.FullyVisible)
                        {
                            val1_2 = firstVisibleLine.Height;
                        }
                        else
                        {
                            val1_2 = firstVisibleLine.Bottom - _textView.ViewportTop;
                            var containingBufferPosition = textViewLines.GetTextViewLineContainingBufferPosition(firstVisibleLine.EndIncludingLineBreak);
                            if (containingBufferPosition != null)
                                val1_2 += containingBufferPosition.Height;
                        }
                        pixelsToScroll = -Math.Min(val1_2, _textView.ViewportHeight);
                    }
                    ScrollViewportVerticallyByPixels(pixelsToScroll);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        public void ScrollViewportVerticallyByLines(ScrollDirection direction, int count)
        {
            switch (direction)
            {
                case ScrollDirection.Up:
                case ScrollDirection.Down:
                    if (count < 0)
                        throw new ArgumentOutOfRangeException(nameof(count));
                    for (var index = 0; index < count; ++index)
                        ScrollViewportVerticallyByLine(direction);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        public bool ScrollViewportVerticallyByPage(ScrollDirection direction)
        {
            switch (direction)
            {
                case ScrollDirection.Up:
                case ScrollDirection.Down:
                    var textViewLines1 = _textView.TextViewLines;
                    if (textViewLines1 != null)
                    {
                        if (direction == ScrollDirection.Up)
                        {
                            var firstVisibleLine1 = textViewLines1.FirstVisibleLine;
                            var start = firstVisibleLine1.Start;
                            if (firstVisibleLine1.VisibilityState == VisibilityState.FullyVisible)
                            {
                                if (start > 0)
                                    start = textViewLines1.GetTextViewLineContainingBufferPosition(start - 1).Start;
                            }
                            else if (_textView.ViewportHeight < firstVisibleLine1.Height)
                            {
                                ScrollViewportVerticallyByPixels(_textView.ViewportHeight);
                                return false;
                            }
                            _textView.DisplayTextLineContainingBufferPosition(start, 0.0, ViewRelativePosition.Bottom);
                            var textViewLines2 = _textView.TextViewLines;
                            var firstVisibleLine2 = textViewLines2.FirstVisibleLine;
                            if (firstVisibleLine2.VisibilityState != VisibilityState.FullyVisible)
                            {
                                var textViewLine = textViewLines2[textViewLines2.Count - 1];
                                if (textViewLine.VisibilityState == VisibilityState.FullyVisible)
                                    _textView.DisplayTextLineContainingBufferPosition(firstVisibleLine2.EndIncludingLineBreak, 0.0, ViewRelativePosition.Top);
                                else if (textViewLine.Bottom - _textView.ViewportBottom > firstVisibleLine2.Bottom - _textView.ViewportTop)
                                    _textView.DisplayTextLineContainingBufferPosition(firstVisibleLine2.EndIncludingLineBreak, 0.0, ViewRelativePosition.Top);
                            }
                        }
                        else
                        {
                            var lastVisibleLine = textViewLines1.LastVisibleLine;
                            var start = lastVisibleLine.Start;
                            if (lastVisibleLine.VisibilityState == VisibilityState.FullyVisible)
                                start = textViewLines1.GetTextViewLineContainingBufferPosition(lastVisibleLine.EndIncludingLineBreak).Start;
                            else if (_textView.ViewportHeight < lastVisibleLine.Height)
                            {
                                ScrollViewportVerticallyByPixels(_textView.ViewportHeight);
                                return false;
                            }
                            _textView.DisplayTextLineContainingBufferPosition(start, 0.0, ViewRelativePosition.Top);
                        }
                    }
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        public void ScrollViewportHorizontallyByPixels(double pixelsToScroll)
        {
            if (double.IsNaN(pixelsToScroll))
                throw new ArgumentOutOfRangeException(nameof(pixelsToScroll));
            _textView.ViewportLeft += pixelsToScroll;
        }

        public void EnsureSpanVisible(SnapshotSpan span)
        {
            EnsureSpanVisible(new VirtualSnapshotSpan(span), EnsureSpanVisibleOptions.None);
        }

        public void EnsureSpanVisible(SnapshotSpan span, EnsureSpanVisibleOptions options)
        {
            EnsureSpanVisible(new VirtualSnapshotSpan(span), options);
        }

        public void EnsureSpanVisible(VirtualSnapshotSpan span, EnsureSpanVisibleOptions options)
        {
            if (span.Snapshot != _textView.TextSnapshot)
                throw new ArgumentException("span is not of the ITextView's TextSnapshot");
            _textView.DoActionThatShouldOnlyBeDoneAfterViewIsLoaded(() => InnerEnsureSpanVisible(span, options));
        }

        private void InnerEnsureSpanVisible(VirtualSnapshotSpan span, EnsureSpanVisibleOptions options)
        {
            if (_textView.IsClosed)
                return;
            if ((options & ~(EnsureSpanVisibleOptions.ShowStart | EnsureSpanVisibleOptions.MinimumScroll | EnsureSpanVisibleOptions.AlwaysCenter)) != EnsureSpanVisibleOptions.None)
                throw new ArgumentOutOfRangeException(nameof(options));
            span = span.TranslateTo(_textView.TextSnapshot);
            EnsureSpanFormatted(span.SnapshotSpan, options);
            if (_textView.ViewportWidth == 0.0)
                return;
            var num1 = Math.Min(4.0, _textView.ViewportWidth / 3.0);
            if ((_textView.Options.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId) & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap)
                return;
            var num2 = double.MaxValue;
            var num3 = double.MinValue;
            var snapshotSpan = span.SnapshotSpan;
            if (snapshotSpan.Length > 0)
            {
                foreach (var normalizedTextBound in _textView.TextViewLines.GetNormalizedTextBounds(span.SnapshotSpan))
                {
                    if (normalizedTextBound.Left < num2)
                        num2 = normalizedTextBound.Left;
                    if (normalizedTextBound.Right > num3)
                        num3 = normalizedTextBound.Right;
                }
            }
            snapshotSpan = span.SnapshotSpan;
            VirtualSnapshotPoint virtualSnapshotPoint;
            if (snapshotSpan.Length != 0)
            {
                virtualSnapshotPoint = span.Start;
                if (!virtualSnapshotPoint.IsInVirtualSpace)
                    goto label_23;
            }
            var textView1 = _textView;
            virtualSnapshotPoint = span.Start;
            var position1 = virtualSnapshotPoint.Position;
            var leading1 = textView1.GetTextViewLineContainingBufferPosition(position1).GetExtendedCharacterBounds(span.Start).Leading;
            if (leading1 < num2)
                num2 = leading1;
            if (leading1 > num3)
                num3 = leading1;
            label_23:
            virtualSnapshotPoint = span.End;
            if (virtualSnapshotPoint.IsInVirtualSpace)
            {
                var textView2 = _textView;
                virtualSnapshotPoint = span.End;
                var position2 = virtualSnapshotPoint.Position;
                var leading2 = textView2.GetTextViewLineContainingBufferPosition(position2).GetExtendedCharacterBounds(span.End).Leading;
                if (leading2 < num2)
                    num2 = leading2;
                if (leading2 > num3)
                    num3 = leading2;
            }
            var val1_1 = num2 - num1;
            var val1_2 = num3 + num1;
            if (_textView.ViewportWidth < val1_2 - val1_1)
            {
                var bufferPosition = (options & EnsureSpanVisibleOptions.ShowStart) != EnsureSpanVisibleOptions.None ? span.Start : span.End;
                var leading2 = _textView.GetTextViewLineContainingBufferPosition(bufferPosition.Position).GetExtendedCharacterBounds(bufferPosition).Leading;
                var num4 = Math.Max(val1_1, leading2 + num1 - _textView.ViewportWidth);
                var num5 = Math.Min(val1_2, leading2 - num1 + _textView.ViewportWidth);
                if (_textView.ViewportLeft < num4)
                {
                    _textView.ViewportLeft = num4;
                }
                else
                {
                    if (_textView.ViewportRight <= num5)
                        return;
                    _textView.ViewportLeft = num5 - _textView.ViewportWidth;
                }
            }
            else if (_textView.ViewportLeft > val1_1)
            {
                _textView.ViewportLeft = val1_1;
            }
            else
            {
                if (_textView.ViewportRight >= val1_2)
                    return;
                _textView.ViewportLeft = val1_2 - _textView.ViewportWidth;
            }
        }

        private void EnsureSpanFormatted(SnapshotSpan span, EnsureSpanVisibleOptions options)
        {
            var flag1 = true;
            var flag2 = true;
            var num = 0.0;
            var bufferPosition = span.Start;
            do
            {
                var containingBufferPosition = _textView.GetTextViewLineContainingBufferPosition(bufferPosition);
                num += containingBufferPosition.Height;
                if (num > _textView.ViewportHeight)
                {
                    if ((options & EnsureSpanVisibleOptions.ShowStart) != EnsureSpanVisibleOptions.None)
                    {
                        _textView.DisplayTextLineContainingBufferPosition(span.Start, 0.0, ViewRelativePosition.Top);
                        return;
                    }
                    _textView.DisplayTextLineContainingBufferPosition(span.End, 0.0, ViewRelativePosition.Bottom);
                    return;
                }
                flag2 = flag2 && containingBufferPosition.VisibilityState == VisibilityState.FullyVisible;
                flag1 = flag1 && (containingBufferPosition.VisibilityState == VisibilityState.Unattached || containingBufferPosition.VisibilityState == VisibilityState.Hidden);
                bufferPosition = containingBufferPosition.EndIncludingLineBreak;
            }
            while (bufferPosition.Position < span.End);
            if ((options & EnsureSpanVisibleOptions.AlwaysCenter) != EnsureSpanVisibleOptions.None || flag1 && (options & EnsureSpanVisibleOptions.MinimumScroll) == EnsureSpanVisibleOptions.None)
            {
                _textView.DisplayTextLineContainingBufferPosition(span.Start, (_textView.ViewportHeight - num) * 0.5, ViewRelativePosition.Top);
            }
            else
            {
                if (flag2)
                    return;
                if (span.Start.Position < _textView.TextViewLines.FormattedSpan.Start)
                    _textView.DisplayTextLineContainingBufferPosition(span.Start, 0.0, ViewRelativePosition.Top);
                else
                    _textView.DisplayTextLineContainingBufferPosition(span.End, 0.0, ViewRelativePosition.Bottom);
            }
        }
    }
}