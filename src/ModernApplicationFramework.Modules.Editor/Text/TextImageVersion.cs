﻿using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class TextImageVersion : ITextImageVersion
    {
        public INormalizedTextChangeCollection Changes { get; private set; }

        public object Identifier { get; }

        public int Length { get; private set; }

        public ITextImageVersion Next { get; private set; }

        public int ReiteratedVersionNumber { get; }

        public int VersionNumber { get; }

        public TextImageVersion(int length)
            : this(0, 0, length, new object())
        {
        }

        private TextImageVersion(int versionNumber, int reiteratedVersionNumber, int length, object identifier)
        {
            VersionNumber = versionNumber;
            ReiteratedVersionNumber = reiteratedVersionNumber;
            Identifier = identifier;
            Length = length;
        }

        public int TrackTo(VersionedPosition other, PointTrackingMode mode)
        {
            if (other.Version == null)
                throw new ArgumentException(nameof(other));
            if (other.Version.VersionNumber == VersionNumber)
                return other.Position;
            if (other.Version.VersionNumber > VersionNumber)
                return Tracking.TrackPositionForwardInTime(mode, other.Position, this, other.Version);
            return Tracking.TrackPositionBackwardInTime(mode, other.Position, this, other.Version);
        }

        public Span TrackTo(VersionedSpan span, SpanTrackingMode mode)
        {
            if (span.Version == null)
                throw new ArgumentException(nameof(span));
            if (span.Version.VersionNumber == VersionNumber)
                return span.Span;
            if (span.Version.VersionNumber > VersionNumber)
                return Tracking.TrackSpanForwardInTime(mode, span.Span, this, span.Version);
            return Tracking.TrackSpanBackwardInTime(mode, span.Span, this, span.Version);
        }

        internal TextImageVersion CreateNext(int reiteratedVersionNumber, int length,
            INormalizedTextChangeCollection changes)
        {
            var versionNumber = VersionNumber + 1;
            if (reiteratedVersionNumber < 0)
                reiteratedVersionNumber =
                    changes == null || changes.Count != 0 ? versionNumber : ReiteratedVersionNumber;
            else if (reiteratedVersionNumber > versionNumber)
                throw new ArgumentOutOfRangeException(nameof(reiteratedVersionNumber));
            if (length == -1)
            {
                length = Length;
                var count = changes.Count;
                for (var index = 0; index < count; ++index)
                    length += changes[index].Delta;
            }

            var textImageVersion = new TextImageVersion(versionNumber, reiteratedVersionNumber, length, Identifier);
            SetChanges(changes);
            Next = textImageVersion;
            return textImageVersion;
        }

        internal void SetChanges(INormalizedTextChangeCollection changes)
        {
            if (Changes != null)
                throw new InvalidOperationException("Not allowed to SetChanges twice");
            Changes = changes;
        }

        internal void SetLength(int length)
        {
            if (Length != 0)
                throw new InvalidOperationException("Not allowed to SetLength twice");
            Length = length;
        }
    }
}