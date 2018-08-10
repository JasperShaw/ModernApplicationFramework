using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ModernApplicationFramework.Text.Data
{
    public class NormalizedSpanCollection : ReadOnlyCollection<Span>
    {
        public static readonly NormalizedSpanCollection Empty = new NormalizedSpanCollection();

        public NormalizedSpanCollection()
            : base(new List<Span>(0))
        {
        }

        public NormalizedSpanCollection(Span span)
            : base(ListFromSpan(span))
        {
        }

        public NormalizedSpanCollection(IEnumerable<Span> spans)
            : base(NormalizeSpans(spans))
        {
        }

        private NormalizedSpanCollection(IList<Span> normalizedSpans)
            : base(normalizedSpans)
        {
        }

        internal static NormalizedSpanCollection CreateFromNormalizedSpans(IList<Span> alreadyNormalizedSpans)
        {
            return new NormalizedSpanCollection(alreadyNormalizedSpans);
        }

        public static NormalizedSpanCollection Union(NormalizedSpanCollection left, NormalizedSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0)
                return right;
            if (right.Count == 0)
                return left;
            List<Span> spanList = new List<Span>();
            int index1 = 0;
            int index2 = 0;
            int start = -1;
            int maxValue = int.MaxValue;
            while (index1 < left.Count && index2 < right.Count)
            {
                Span span1 = left[index1];
                Span span2 = right[index2];
                if (span1.Start < span2.Start)
                {
                    UpdateSpanUnion(span1, spanList, ref start, ref maxValue);
                    ++index1;
                }
                else
                {
                    UpdateSpanUnion(span2, spanList, ref start, ref maxValue);
                    ++index2;
                }
            }
            for (; index1 < left.Count; ++index1)
                UpdateSpanUnion(left[index1], spanList, ref start, ref maxValue);
            for (; index2 < right.Count; ++index2)
                UpdateSpanUnion(right[index2], spanList, ref start, ref maxValue);
            if (maxValue != int.MaxValue)
                spanList.Add(Span.FromBounds(start, maxValue));
            return CreateFromNormalizedSpans(spanList);
        }

        public static NormalizedSpanCollection Overlap(NormalizedSpanCollection left, NormalizedSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0)
                return left;
            if (right.Count == 0)
                return right;
            List<Span> spanList = new List<Span>();
            int index1 = 0;
            int index2 = 0;
            while (index1 < left.Count && index2 < right.Count)
            {
                Span span1 = left[index1];
                Span span2 = right[index2];
                if (span1.OverlapsWith(span2))
                    spanList.Add(span1.Overlap(span2).Value);
                if (span1.End < span2.End)
                    ++index1;
                else if (span1.End == span2.End)
                {
                    ++index1;
                    ++index2;
                }
                else
                    ++index2;
            }
            return CreateFromNormalizedSpans(spanList);
        }

        public static NormalizedSpanCollection Intersection(NormalizedSpanCollection left, NormalizedSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0)
                return left;
            if (right.Count == 0)
                return right;
            List<Span> spanList = new List<Span>();
            int index1 = 0;
            int index2 = 0;
            while (index1 < left.Count && index2 < right.Count)
            {
                Span span1 = left[index1];
                Span span2 = right[index2];
                if (span1.IntersectsWith(span2))
                    spanList.Add(span1.Intersection(span2).Value);
                if (span1.End < span2.End)
                    ++index1;
                else
                    ++index2;
            }
            return CreateFromNormalizedSpans(spanList);
        }

        public static NormalizedSpanCollection Difference(NormalizedSpanCollection left, NormalizedSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0 || right.Count == 0)
                return left;
            List<Span> spanList = new List<Span>();
            int index1 = 0;
            int index2 = 0;
            int val1 = -1;
            do
            {
                Span span1 = left[index1];
                Span span2 = right[index2];
                if (span2.Length == 0 || span1.Start >= span2.End)
                    ++index2;
                else if (span1.End <= span2.Start)
                {
                    spanList.Add(Span.FromBounds(Math.Max(val1, span1.Start), span1.End));
                    ++index1;
                }
                else
                {
                    if (span1.Start < span2.Start)
                        spanList.Add(Span.FromBounds(Math.Max(val1, span1.Start), span2.Start));
                    if (span1.End < span2.End)
                        ++index1;
                    else if (span1.End == span2.End)
                    {
                        ++index1;
                        ++index2;
                    }
                    else
                    {
                        val1 = span2.End;
                        ++index2;
                    }
                }
            }
            while (index1 < left.Count && index2 < right.Count);
            while (index1 < left.Count)
            {
                Span span = left[index1++];
                spanList.Add(Span.FromBounds(Math.Max(val1, span.Start), span.End));
            }
            return CreateFromNormalizedSpans(spanList);
        }

        public static bool operator ==(NormalizedSpanCollection left, NormalizedSpanCollection right)
        {
            if (left == (object)right)
                return true;
            if ((object)left == null || (object)right == null || left.Count != right.Count)
                return false;
            return !left.Where((t, index) => t != right[index]).Any();
        }

        public static bool operator !=(NormalizedSpanCollection left, NormalizedSpanCollection right)
        {
            return !(left == right);
        }

        public bool OverlapsWith(NormalizedSpanCollection set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));
            int index1 = 0;
            int index2 = 0;
            while (index1 < Count && index2 < set.Count)
            {
                Span span1 = this[index1];
                Span span2 = set[index2];
                if (span1.OverlapsWith(span2))
                    return true;
                if (span1.End < span2.End)
                    ++index1;
                else if (span1.End == span2.End)
                {
                    ++index1;
                    ++index2;
                }
                else
                    ++index2;
            }
            return false;
        }

        public bool OverlapsWith(Span span)
        {
            for (int index = 0; index < Count; ++index)
            {
                if (this[index].OverlapsWith(span))
                    return true;
            }
            return false;
        }

        public bool IntersectsWith(NormalizedSpanCollection set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));
            int index1 = 0;
            int index2 = 0;
            while (index1 < Count && index2 < set.Count)
            {
                Span span1 = this[index1];
                Span span2 = set[index2];
                if (span1.IntersectsWith(span2))
                    return true;
                if (span1.End < span2.End)
                    ++index1;
                else
                    ++index2;
            }
            return false;
        }

        public bool IntersectsWith(Span span)
        {
            for (int index = 0; index < Count; ++index)
            {
                if (this[index].IntersectsWith(span))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int num = 0;
            foreach (Span span in this)
                num ^= span.GetHashCode();
            return num;
        }

        public override bool Equals(object obj)
        {
            return this == obj as NormalizedSpanCollection;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder("{");
            foreach (Span span in this)
                stringBuilder.Append(span.ToString());
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        private static IList<Span> ListFromSpan(Span span)
        {
            var spanList = new List<Span>(1) {span};
            return spanList;
        }

        private static void UpdateSpanUnion(Span span, IList<Span> spans, ref int start, ref int end)
        {
            if (end < span.Start)
            {
                spans.Add(Span.FromBounds(start, end));
                start = -1;
                end = int.MaxValue;
            }
            if (end == int.MaxValue)
            {
                start = span.Start;
                end = span.End;
            }
            else
                end = Math.Max(end, span.End);
        }

        private static IList<Span> NormalizeSpans(IEnumerable<Span> spans)
        {
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            List<Span> spanList1 = new List<Span>(spans);
            if (spanList1.Count <= 1)
                return spanList1;
            spanList1.Sort(SpanStartComparer.Default);
            int num1 = 0;
            int start1 = spanList1[0].Start;
            Span span1 = spanList1[0];
            int num2 = span1.End;
            for (int index = 1; index < spanList1.Count; ++index)
            {
                span1 = spanList1[index];
                int start2 = span1.Start;
                span1 = spanList1[index];
                int end = span1.End;
                if (num2 < start2)
                {
                    spanList1[num1++] = Span.FromBounds(start1, num2);
                    start1 = start2;
                    num2 = end;
                }
                else
                    num2 = Math.Max(num2, end);
            }
            List<Span> spanList2 = spanList1;
            int index1 = num1;
            int index2 = index1 + 1;
            Span span2 = Span.FromBounds(start1, num2);
            spanList2[index1] = span2;
            spanList1.RemoveRange(index2, spanList1.Count - index2);
            if (spanList1.Capacity > 10)
                spanList1.TrimExcess();
            return spanList1;
        }

        private class SpanStartComparer : IComparer<Span>
        {
            public static readonly IComparer<Span> Default = new SpanStartComparer();

            public int Compare(Span s1, Span s2)
            {
                return s1.Start.CompareTo(s2.Start);
            }
        }
    }
}