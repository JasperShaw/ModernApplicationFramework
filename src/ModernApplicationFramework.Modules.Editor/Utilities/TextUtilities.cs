using System;
using System.Collections.Generic;
using System.Text;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal static class TextUtilities
    {
        public static int LengthOfLineBreak(string source, int start, int end)
        {
            switch (source[start])
            {
                case '\n':
                case '\x0085':
                case '\x2028':
                case '\x2029':
                    return 1;
                case '\r':
                    return ++start >= end || source[start] != '\n' ? 1 : 2;
                default:
                    return 0;
            }
        }

        public static int LengthOfLineBreak(char[] source, int start, int end)
        {
            switch (source[start])
            {
                case '\n':
                case '\x0085':
                case '\x2028':
                case '\x2029':
                    return 1;
                case '\r':
                    return ++start >= end || source[start] != '\n' ? 1 : 2;
                default:
                    return 0;
            }
        }

        public static int ScanForLineCount(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            var num1 = 0;
            var start = 0;
            while (start < text.Length)
            {
                var num2 = LengthOfLineBreak(text, start, text.Length);
                if (num2 > 0)
                {
                    ++num1;
                    start += num2;
                }
                else
                    ++start;
            }
            return num1;
        }

        public static Span? Overlap(this Span span, Span? otherSpan)
        {
            return !otherSpan.HasValue ? new Span?() : span.Overlap(otherSpan.Value);
        }

        public static string GetTagOrContentType(ITextBuffer buffer)
        {
            return GetTag(buffer) ?? buffer.ContentType.TypeName;
        }

        public static string GetTag(ITextBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            var property = "";
            buffer.Properties.TryGetProperty("tag", out property);
            return property;
        }

        public static T[] StableSort<T>(IReadOnlyList<T> elements, Comparison<T> comparer)
        {
            var count = elements.Count;
            var target = new T[count];
            var flag = true;
            if (count > 0)
            {
                target[0] = elements[0];
                for (var index = 1; index < count; ++index)
                {
                    target[index] = elements[index];
                    if (comparer(target[index - 1], target[index]) > 0)
                        flag = false;
                }
            }
            if (flag)
                return target;
            var source = new T[count];
            var subcount = 1;
            while (subcount < count)
            {
                var objArray = source;
                source = target;
                target = objArray;
                MergePass(source, target, subcount, comparer);
                subcount *= 2;
            }
            return target;
        }

        private static void MergePass<T>(T[] source, T[] target, int subcount, Comparison<T> comparer)
        {
            var left = 0;
            while (left <= source.Length - 2 * subcount)
            {
                Merge(source, target, left, left + subcount, left + 2 * subcount, comparer);
                left += 2 * subcount;
            }
            if (left + subcount < source.Length)
            {
                Merge(source, target, left, left + subcount, source.Length, comparer);
            }
            else
            {
                for (var index = left; index < source.Length; ++index)
                    target[index] = source[index];
            }
        }

        private static void Merge<T>(T[] source, T[] target, int left, int right, int end, Comparison<T> comparer)
        {
            var index1 = left;
            var index2 = right;
            var num = left;
            while (index1 < right && index2 < end)
                target[num++] = comparer(source[index1], source[index2]) <= 0 ? source[index1++] : source[index2++];
            if (index1 == right)
            {
                for (var index3 = num; index3 < end; ++index3)
                    target[index3] = source[index2++];
            }
            else
            {
                for (var index3 = num; index3 < end; ++index3)
                    target[index3] = source[index1++];
            }
        }

        public static string Escape(string text, int truncateLength)
        {
            var result = new StringBuilder();
            if (text.Length <= truncateLength)
                return Escape(text);
            for (var index = 0; index < truncateLength / 2; ++index)
            {
                var ch = text[index];
                AppendChar(result, ch);
            }
            result.Append(" � ");
            for (var index = text.Length - truncateLength / 2; index < text.Length; ++index)
            {
                var ch = text[index];
                AppendChar(result, ch);
            }
            return result.ToString();
        }

        public static string Escape(string text)
        {
            var result = new StringBuilder();
            foreach (var ch in text)
            {
                AppendChar(result, ch);
            }
            return result.ToString();
        }

        private static void AppendChar(StringBuilder result, char ch)
        {
            switch (ch)
            {
                case '\t':
                    result.Append("\\t");
                    break;
                case '\n':
                    result.Append("\\n");
                    break;
                case '\r':
                    result.Append("\\r");
                    break;
                case '"':
                    result.Append("\\\"");
                    break;
                case '\\':
                    result.Append("\\\\");
                    break;
                default:
                    result.Append(ch);
                    break;
            }
        }
    }
}