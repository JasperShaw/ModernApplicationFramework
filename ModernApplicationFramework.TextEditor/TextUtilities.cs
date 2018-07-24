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
    }
}