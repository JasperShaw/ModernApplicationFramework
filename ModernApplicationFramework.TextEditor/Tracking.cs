using System;

namespace ModernApplicationFramework.TextEditor
{
    public static class Tracking
    {
        public static int TrackPositionForwardInTime(PointTrackingMode trackingMode, int currentPosition, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.TextBuffer != currentVersion.TextBuffer)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same TextBuffer");
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    for (; currentVersion != targetVersion; currentVersion = currentVersion.Next)
                        currentPosition = TrackPositionForwardInTime(trackingMode, currentPosition, currentVersion.Changes);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static Span TrackSpanBackwardInTime(SpanTrackingMode trackingMode, Span span, ITextImageVersion currentVersion, ITextImageVersion targetVersion)
        {
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                case SpanTrackingMode.Custom:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.Identifier != currentVersion.Identifier)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same ITextImage");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    int num = TrackPositionBackwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive ? PointTrackingMode.Positive : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num, Math.Max(num, TrackPositionBackwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static int TrackPositionBackwardInTime(PointTrackingMode trackingMode, int currentPosition, ITextImageVersion currentVersion, ITextImageVersion targetVersion)
        {
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.Identifier != currentVersion.Identifier)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same ITextImage");
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    INormalizedTextChangeCollection[] changeCollectionArray = new INormalizedTextChangeCollection[currentVersion.VersionNumber - targetVersion.VersionNumber];
                    int num = 0;
                    for (ITextImageVersion textImageVersion = targetVersion; textImageVersion != currentVersion; textImageVersion = textImageVersion.Next)
                        changeCollectionArray[num++] = textImageVersion.Changes;
                    while (num > 0)
                        currentPosition = TrackPositionBackwardInTime(trackingMode, currentPosition, changeCollectionArray[--num]);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static Span TrackSpanBackwardInTime(SpanTrackingMode trackingMode, Span span, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                case SpanTrackingMode.Custom:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.TextBuffer != currentVersion.TextBuffer)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same TextBuffer");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    int num = TrackPositionBackwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive ? PointTrackingMode.Positive : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num, Math.Max(num, TrackPositionBackwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static int TrackPositionBackwardInTime(PointTrackingMode trackingMode, int currentPosition, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.TextBuffer != currentVersion.TextBuffer)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same TextBuffer");
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    INormalizedTextChangeCollection[] changeCollectionArray = new INormalizedTextChangeCollection[currentVersion.VersionNumber - targetVersion.VersionNumber];
                    int num = 0;
                    for (ITextVersion textVersion = targetVersion; textVersion != currentVersion; textVersion = textVersion.Next)
                        changeCollectionArray[num++] = textVersion.Changes;
                    while (num > 0)
                        currentPosition = TrackPositionBackwardInTime(trackingMode, currentPosition, changeCollectionArray[--num]);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static int TrackPositionBackwardInTime(PointTrackingMode trackingMode, int currentPosition, INormalizedTextChangeCollection textChanges)
        {
            int num = 0;
            int index1 = textChanges.Count - 1;
            while (num <= index1)
            {
                int index2 = (num + index1) / 2;
                ITextChange textChange = textChanges[index2];
                if (currentPosition < textChange.NewPosition)
                    index1 = index2 - 1;
                else if (currentPosition > textChange.NewEnd)
                {
                    num = index2 + 1;
                }
                else
                {
                    if (IsOpaque(textChange))
                    {
                        int offset = currentPosition - textChange.NewPosition;
                        if (offset > 0)
                        {
                            if (offset >= textChange.OldLength || offset >= textChange.NewLength)
                                offset = textChange.OldLength;
                            else if (ShouldOffsetEndpointOfChange(textChange, offset, false))
                                --offset;
                        }
                        currentPosition = textChange.OldPosition + offset;
                        break;
                    }
                    currentPosition = trackingMode != PointTrackingMode.Positive ? textChange.OldPosition : textChange.OldEnd;
                    break;
                }
            }
            if (index1 < num && num > 0)
            {
                ITextChange textChange = textChanges[index1];
                currentPosition += textChange.OldEnd - textChange.NewEnd;
            }
            return currentPosition;
        }

        public static int TrackPositionForwardInTime(PointTrackingMode trackingMode, int currentPosition, INormalizedTextChangeCollection textChanges)
        {
            int num = 0;
            int index1 = textChanges.Count - 1;
            while (num <= index1)
            {
                int index2 = (num + index1) / 2;
                ITextChange textChange = textChanges[index2];
                if (currentPosition < textChange.OldPosition)
                    index1 = index2 - 1;
                else if (currentPosition > textChange.OldEnd)
                {
                    num = index2 + 1;
                }
                else
                {
                    if (IsOpaque(textChange))
                    {
                        int offset = currentPosition - textChange.OldPosition;
                        if (offset > 0)
                        {
                            if (offset >= textChange.OldLength || offset >= textChange.NewLength)
                                offset = textChange.NewLength;
                            else if (ShouldOffsetEndpointOfChange(textChange, offset, true))
                                --offset;
                        }
                        currentPosition = textChange.NewPosition + offset;
                        break;
                    }
                    currentPosition = trackingMode != PointTrackingMode.Positive ? textChange.NewPosition : textChange.NewEnd;
                    break;
                }
            }
            if (index1 < num && num > 0)
            {
                ITextChange textChange = textChanges[index1];
                currentPosition += textChange.NewEnd - textChange.OldEnd;
            }
            return currentPosition;
        }

        private static bool ShouldOffsetEndpointOfChange(ITextChange textChange, int offset, bool isForwardTracking)
        {
            ITextChange3 textChange3 = textChange as ITextChange3;
            if (isForwardTracking)
            {
                if (textChange3 == null)
                {
                    if (textChange.NewText[offset] != '\n' || textChange.NewText[offset - 1] != '\r')
                        return false;
                    if (textChange.OldText[offset] == '\n')
                        return textChange.OldText[offset - 1] != '\r';
                    return true;
                }
                if (textChange3.GetNewTextAt(offset) != '\n' || textChange3.GetNewTextAt(offset - 1) != '\r')
                    return false;
                if (textChange3.GetOldTextAt(offset) == '\n')
                    return textChange3.GetOldTextAt(offset - 1) != '\r';
                return true;
            }
            if (textChange3 == null)
            {
                if (textChange.OldText[offset] != '\n' || textChange.OldText[offset - 1] != '\r')
                    return false;
                if (textChange.NewText[offset] == '\n')
                    return textChange.NewText[offset - 1] != '\r';
                return true;
            }
            if (textChange3.GetOldTextAt(offset) != '\n' || textChange3.GetOldTextAt(offset - 1) != '\r')
                return false;
            if (textChange3.GetNewTextAt(offset) == '\n')
                return textChange3.GetNewTextAt(offset - 1) != '\r';
            return true;
        }

        public static Span TrackSpanForwardInTime(SpanTrackingMode trackingMode, Span span, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                case SpanTrackingMode.Custom:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.TextBuffer != currentVersion.TextBuffer)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same TextBuffer");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    int num = TrackPositionForwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive ? PointTrackingMode.Positive : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num, Math.Max(num, TrackPositionForwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static Span TrackSpanForwardInTime(SpanTrackingMode trackingMode, Span span, ITextImageVersion currentVersion, ITextImageVersion targetVersion)
        {
            switch (trackingMode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                case SpanTrackingMode.Custom:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.Identifier != currentVersion.Identifier)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same ITextImage");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    int num = TrackPositionForwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive ? PointTrackingMode.Positive : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num, Math.Max(num, TrackPositionForwardInTime(trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgeNegative ? PointTrackingMode.Negative : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static int TrackPositionForwardInTime(PointTrackingMode trackingMode, int currentPosition, ITextImageVersion currentVersion, ITextImageVersion targetVersion)
        {
            switch (trackingMode)
            {
                case PointTrackingMode.Positive:
                case PointTrackingMode.Negative:
                    if (currentVersion == null)
                        throw new ArgumentNullException(nameof(currentVersion));
                    if (targetVersion == null)
                        throw new ArgumentNullException(nameof(targetVersion));
                    if (targetVersion.Identifier != currentVersion.Identifier)
                        throw new ArgumentException("currentVersion and targetVersion must be from the same ITextImage");
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    for (; currentVersion != targetVersion; currentVersion = currentVersion.Next)
                        currentPosition = TrackPositionForwardInTime(trackingMode, currentPosition, currentVersion.Changes);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        private static bool IsOpaque(ITextChange textChange)
        {
            if (textChange is ITextChange2 textChange2)
                return textChange2.IsOpaque;
            return false;
        }
    }
}