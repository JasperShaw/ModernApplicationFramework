﻿using System;
using System.Globalization;

namespace ModernApplicationFramework.Text.Data
{
    public struct Span
    {
        public int End => Start + Length;

        public bool IsEmpty => Length == 0;
        public int Length { get; }
        public int Start { get; }

        public Span(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (start + length < start)
                throw new ArgumentOutOfRangeException(nameof(length));
            Start = start;
            Length = length;
        }

        public static Span FromBounds(int start, int end)
        {
            return new Span(start, end - start);
        }

        public static bool operator ==(Span left, Span right)
        {
            if (left.Start == right.Start)
                return left.Length == right.Length;
            return false;
        }

        public static bool operator !=(Span left, Span right)
        {
            return !(left == right);
        }

        public bool Contains(int position)
        {
            if (position >= Start)
                return position < End;
            return false;
        }

        public bool Contains(Span span)
        {
            if (span.Start >= Start)
                return span.End <= End;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Span))
                return false;
            var span = (Span) obj;
            if (span.Start == Start)
                return span.Length == Length;
            return false;
        }

        public override int GetHashCode()
        {
            var num = Length;
            var hashCode1 = num.GetHashCode();
            num = Start;
            var hashCode2 = num.GetHashCode();
            return hashCode1 ^ hashCode2;
        }

        public Span? Intersection(Span span)
        {
            var start = Math.Max(Start, span.Start);
            var end = Math.Min(End, span.End);
            return start <= end ? FromBounds(start, end) : new Span?();
        }

        public bool IntersectsWith(Span span)
        {
            if (span.Start <= End)
                return span.End >= Start;
            return false;
        }

        public Span? Overlap(Span span)
        {
            var start = Math.Max(Start, span.Start);
            var end = Math.Min(End, span.End);
            return start < end ? FromBounds(start, end) : new Span?();
        }

        public bool OverlapsWith(Span span)
        {
            return Math.Max(Start, span.Start) < Math.Min(End, span.End);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0}..{1})", Start, Start + Length);
        }
    }
}