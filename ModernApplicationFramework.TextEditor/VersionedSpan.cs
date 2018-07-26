﻿using System;
using System.Globalization;

namespace ModernApplicationFramework.TextEditor
{
    public struct VersionedSpan : IEquatable<VersionedSpan>
    {
        public readonly ITextImageVersion Version;
        public readonly Span Span;
        public static readonly VersionedSpan Invalid;

        public VersionedSpan(ITextImageVersion version, Span span)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (span.End > version.Length)
                throw new ArgumentOutOfRangeException(nameof(span));
            Version = version;
            Span = span;
        }

        public static implicit operator Span(VersionedSpan span)
        {
            return span.Span;
        }

        public VersionedSpan TranslateTo(ITextImageVersion other, SpanTrackingMode mode)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            return new VersionedSpan(other, other.TrackTo(this, mode));
        }

        public override int GetHashCode()
        {
            if (Version == null)
                return 0;
            return Span.GetHashCode() ^ Version.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is VersionedSpan)
                return Equals((VersionedSpan)obj);
            return false;
        }

        public bool Equals(VersionedSpan other)
        {
            if (other.Version == Version)
                return other.Span == Span;
            return false;
        }

        public static bool operator ==(VersionedSpan left, VersionedSpan right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VersionedSpan left, VersionedSpan right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return Version != null ? string.Format(CultureInfo.CurrentCulture, "v{0}_{1}", Version.VersionNumber, Span) : "Invalid";
        }
    }
}