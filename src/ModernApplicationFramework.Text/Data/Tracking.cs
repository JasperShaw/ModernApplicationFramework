using System;

namespace ModernApplicationFramework.Text.Data
{
    public static class Tracking
    {
        public static int TrackPositionBackwardInTime(PointTrackingMode trackingMode, int currentPosition,
            ITextImageVersion currentVersion, ITextImageVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same ITextImage");
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    var changeCollectionArray =
                        new INormalizedTextChangeCollection[currentVersion.VersionNumber - targetVersion.VersionNumber];
                    var num = 0;
                    for (var textImageVersion = targetVersion;
                        textImageVersion != currentVersion;
                        textImageVersion = textImageVersion.Next)
                        changeCollectionArray[num++] = textImageVersion.Changes;
                    while (num > 0)
                        currentPosition = TrackPositionBackwardInTime(trackingMode, currentPosition,
                            changeCollectionArray[--num]);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static int TrackPositionBackwardInTime(PointTrackingMode trackingMode, int currentPosition,
            ITextVersion currentVersion, ITextVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same TextBuffer");
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    var changeCollectionArray =
                        new INormalizedTextChangeCollection[currentVersion.VersionNumber - targetVersion.VersionNumber];
                    var num = 0;
                    for (var textVersion = targetVersion; textVersion != currentVersion; textVersion = textVersion.Next)
                        changeCollectionArray[num++] = textVersion.Changes;
                    while (num > 0)
                        currentPosition = TrackPositionBackwardInTime(trackingMode, currentPosition,
                            changeCollectionArray[--num]);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static int TrackPositionBackwardInTime(PointTrackingMode trackingMode, int currentPosition,
            INormalizedTextChangeCollection textChanges)
        {
            var num = 0;
            var index1 = textChanges.Count - 1;
            while (num <= index1)
            {
                var index2 = (num + index1) / 2;
                var textChange = textChanges[index2];
                if (currentPosition < textChange.NewPosition)
                {
                    index1 = index2 - 1;
                }
                else if (currentPosition > textChange.NewEnd)
                {
                    num = index2 + 1;
                }
                else
                {
                    if (IsOpaque(textChange))
                    {
                        var offset = currentPosition - textChange.NewPosition;
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

                    currentPosition = trackingMode != PointTrackingMode.Positive
                        ? textChange.OldPosition
                        : textChange.OldEnd;
                    break;
                }
            }

            if (index1 < num && num > 0)
            {
                var textChange = textChanges[index1];
                currentPosition += textChange.OldEnd - textChange.NewEnd;
            }

            return currentPosition;
        }

        public static int TrackPositionForwardInTime(PointTrackingMode trackingMode, int currentPosition,
            ITextVersion currentVersion, ITextVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same TextBuffer");
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    for (; currentVersion != targetVersion; currentVersion = currentVersion.Next)
                        currentPosition =
                            TrackPositionForwardInTime(trackingMode, currentPosition, currentVersion.Changes);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static int TrackPositionForwardInTime(PointTrackingMode trackingMode, int currentPosition,
            INormalizedTextChangeCollection textChanges)
        {
            var num = 0;
            var index1 = textChanges.Count - 1;
            while (num <= index1)
            {
                var index2 = (num + index1) / 2;
                var textChange = textChanges[index2];
                if (currentPosition < textChange.OldPosition)
                {
                    index1 = index2 - 1;
                }
                else if (currentPosition > textChange.OldEnd)
                {
                    num = index2 + 1;
                }
                else
                {
                    if (IsOpaque(textChange))
                    {
                        var offset = currentPosition - textChange.OldPosition;
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

                    currentPosition = trackingMode != PointTrackingMode.Positive
                        ? textChange.NewPosition
                        : textChange.NewEnd;
                    break;
                }
            }

            if (index1 < num && num > 0)
            {
                var textChange = textChanges[index1];
                currentPosition += textChange.NewEnd - textChange.OldEnd;
            }

            return currentPosition;
        }

        public static int TrackPositionForwardInTime(PointTrackingMode trackingMode, int currentPosition,
            ITextImageVersion currentVersion, ITextImageVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same ITextImage");
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    if (currentPosition < 0 || currentPosition > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(currentPosition));
                    for (; currentVersion != targetVersion; currentVersion = currentVersion.Next)
                        currentPosition =
                            TrackPositionForwardInTime(trackingMode, currentPosition, currentVersion.Changes);
                    return currentPosition;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static Span TrackSpanBackwardInTime(SpanTrackingMode trackingMode, Span span,
            ITextImageVersion currentVersion, ITextImageVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same ITextImage");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    var num = TrackPositionBackwardInTime(
                        trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive
                            ? PointTrackingMode.Positive
                            : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num,
                        Math.Max(num,
                            TrackPositionBackwardInTime(
                                trackingMode == SpanTrackingMode.EdgeExclusive ||
                                trackingMode == SpanTrackingMode.EdgeNegative
                                    ? PointTrackingMode.Negative
                                    : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static Span TrackSpanBackwardInTime(SpanTrackingMode trackingMode, Span span,
            ITextVersion currentVersion, ITextVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same TextBuffer");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber > currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    var num = TrackPositionBackwardInTime(
                        trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive
                            ? PointTrackingMode.Positive
                            : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num,
                        Math.Max(num,
                            TrackPositionBackwardInTime(
                                trackingMode == SpanTrackingMode.EdgeExclusive ||
                                trackingMode == SpanTrackingMode.EdgeNegative
                                    ? PointTrackingMode.Negative
                                    : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static Span TrackSpanForwardInTime(SpanTrackingMode trackingMode, Span span, ITextVersion currentVersion,
            ITextVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same TextBuffer");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    var num = TrackPositionForwardInTime(
                        trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive
                            ? PointTrackingMode.Positive
                            : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num,
                        Math.Max(num,
                            TrackPositionForwardInTime(
                                trackingMode == SpanTrackingMode.EdgeExclusive ||
                                trackingMode == SpanTrackingMode.EdgeNegative
                                    ? PointTrackingMode.Negative
                                    : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingMode));
            }
        }

        public static Span TrackSpanForwardInTime(SpanTrackingMode trackingMode, Span span,
            ITextImageVersion currentVersion, ITextImageVersion targetVersion)
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
                        throw new ArgumentException(
                            "currentVersion and targetVersion must be from the same ITextImage");
                    if (span.End > currentVersion.Length)
                        throw new ArgumentOutOfRangeException(nameof(span));
                    if (targetVersion.VersionNumber < currentVersion.VersionNumber)
                        throw new ArgumentOutOfRangeException(nameof(targetVersion));
                    var num = TrackPositionForwardInTime(
                        trackingMode == SpanTrackingMode.EdgeExclusive || trackingMode == SpanTrackingMode.EdgePositive
                            ? PointTrackingMode.Positive
                            : PointTrackingMode.Negative, span.Start, currentVersion, targetVersion);
                    return Span.FromBounds(num,
                        Math.Max(num,
                            TrackPositionForwardInTime(
                                trackingMode == SpanTrackingMode.EdgeExclusive ||
                                trackingMode == SpanTrackingMode.EdgeNegative
                                    ? PointTrackingMode.Negative
                                    : PointTrackingMode.Positive, span.End, currentVersion, targetVersion)));
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

        private static bool ShouldOffsetEndpointOfChange(ITextChange textChange, int offset, bool isForwardTracking)
        {
            var textChange3 = textChange as ITextChange3;
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
    }
}