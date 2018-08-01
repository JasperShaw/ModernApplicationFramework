using System;
using System.Collections.Generic;
using System.Text;

namespace ModernApplicationFramework.TextEditor
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

        public static string GetTagOrContentType(ITextBuffer buffer)
        {
            return GetTag(buffer) ?? buffer.ContentType.TypeName;
        }

        public static string GetTag(ITextBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            string property = "";
            buffer.Properties.TryGetProperty("tag", out property);
            return property;
        }

        public static T[] StableSort<T>(IReadOnlyList<T> elements, Comparison<T> comparer)
        {
            int count = elements.Count;
            T[] target = new T[count];
            bool flag = true;
            if (count > 0)
            {
                target[0] = elements[0];
                for (int index = 1; index < count; ++index)
                {
                    target[index] = elements[index];
                    if (comparer(target[index - 1], target[index]) > 0)
                        flag = false;
                }
            }
            if (flag)
                return target;
            T[] source = new T[count];
            int subcount = 1;
            while (subcount < count)
            {
                T[] objArray = source;
                source = target;
                target = objArray;
                MergePass(source, target, subcount, comparer);
                subcount *= 2;
            }
            return target;
        }

        private static void MergePass<T>(T[] source, T[] target, int subcount, Comparison<T> comparer)
        {
            int left = 0;
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
                for (int index = left; index < source.Length; ++index)
                    target[index] = source[index];
            }
        }

        private static void Merge<T>(T[] source, T[] target, int left, int right, int end, Comparison<T> comparer)
        {
            int index1 = left;
            int index2 = right;
            int num = left;
            while (index1 < right && index2 < end)
                target[num++] = comparer(source[index1], source[index2]) <= 0 ? source[index1++] : source[index2++];
            if (index1 == right)
            {
                for (int index3 = num; index3 < end; ++index3)
                    target[index3] = source[index2++];
            }
            else
            {
                for (int index3 = num; index3 < end; ++index3)
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