using System;
using System.IO;
using System.Threading;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    internal static class TextImageLoader
    {
        public const int BlockSize = 16384;
        private static char[] _pooledBuffer;

        internal static StringRebuilder Load(TextReader reader, long fileSize, string id, out bool hasConsistentLineEndings, out int longestLineLength, int blockSize = 0, int minCompressedBlockSize = 16384)
        {
            var lineEnding = LineEndingState.Unknown;
            var currentLineLength = 0;
            longestLineLength = 0;
            var flag = fileSize >= TextModelOptions.CompressedStorageFileSizeThreshold;
            if (blockSize == 0)
                blockSize = flag ? TextModelOptions.CompressedStoragePageSize : 16384;
            PageManager manager = null;
            char[] buffer;
            if (flag)
            {
                manager = new PageManager();
                buffer = new char[blockSize];
            }
            else
                buffer = AcquireBuffer(blockSize);
            var stringRebuilder = StringRebuilder.Empty;
            try
            {
                while (true)
                {
                    var length = LoadNextBlock(reader, buffer);
                    if (length != 0)
                    {
                        var lineBreakEditor = LineBreakManager.CreateLineBreakEditor(length, 0);
                        ParseBlock(buffer, length, lineBreakEditor, ref lineEnding, ref currentLineLength, ref longestLineLength);
                        var chArray = buffer;
                        if (length < buffer.Length / 2)
                        {
                            chArray = new char[length];
                            Array.Copy(buffer, chArray, length);
                        }
                        else
                            buffer = new char[blockSize];
                        var text = !flag || length <= minCompressedBlockSize ? StringRebuilderForChars.Create(chArray, length, lineBreakEditor) : StringRebuilderForCompressedChars.Create(new Page(manager, chArray, length), (ILineBreaks)lineBreakEditor);
                        stringRebuilder = stringRebuilder.Insert(stringRebuilder.Length, text);
                    }
                    else
                        break;
                }
                longestLineLength = Math.Max(longestLineLength, currentLineLength);
                hasConsistentLineEndings = lineEnding != LineEndingState.Inconsistent;
            }
            finally
            {
                if (!flag)
                    ReleaseBuffer(buffer);
            }
            return stringRebuilder;
        }

        public static int LoadNextBlock(TextReader reader, char[] buffer)
        {
            var num = reader.ReadBlock(buffer, 0, buffer.Length - 1);
            if (num == buffer.Length - 1 && buffer[num - 1] == '\r' && reader.Peek() == 10)
            {
                reader.Read();
                buffer[num++] = '\n';
            }
            return num;
        }

        private static void ParseBlock(char[] buffer, int length, ILineBreaksEditor lineBreaks, ref LineEndingState lineEnding, ref int currentLineLength, ref int longestLineLength)
        {
            var start = 0;
            while (start < length)
            {
                var length1 = TextUtilities.LengthOfLineBreak(buffer, start, length);
                if (length1 == 0)
                {
                    ++currentLineLength;
                    ++start;
                }
                else
                {
                    lineBreaks.Add(start, length1);
                    longestLineLength = Math.Max(longestLineLength, currentLineLength);
                    currentLineLength = 0;
                    if (lineEnding != LineEndingState.Inconsistent)
                    {
                        if (length1 == 2)
                        {
                            if (lineEnding == LineEndingState.Unknown)
                                lineEnding = LineEndingState.Crlf;
                            else if (lineEnding != LineEndingState.Crlf)
                                lineEnding = LineEndingState.Inconsistent;
                        }
                        else
                        {
                            LineEndingState lineEndingState;
                            switch (buffer[start])
                            {
                                case '\n':
                                    lineEndingState = LineEndingState.Lf;
                                    break;
                                case '\r':
                                    lineEndingState = LineEndingState.Cr;
                                    break;
                                case '\x0085':
                                    lineEndingState = LineEndingState.Nel;
                                    break;
                                case '\x2028':
                                    lineEndingState = LineEndingState.Ls;
                                    break;
                                case '\x2029':
                                    lineEndingState = LineEndingState.Ps;
                                    break;
                                default:
                                    throw new InvalidOperationException("Unexpected line ending");
                            }
                            if (lineEnding == LineEndingState.Unknown)
                                lineEnding = lineEndingState;
                            else if (lineEnding != lineEndingState)
                                lineEnding = LineEndingState.Inconsistent;
                        }
                    }
                    start += length1;
                }
            }
        }

        private static char[] AcquireBuffer(int size)
        {
            var comparand = Volatile.Read(ref _pooledBuffer);
            if (comparand != null && comparand.Length >= size && comparand == Interlocked.CompareExchange(ref _pooledBuffer, null, comparand))
                return comparand;
            return new char[size];
        }

        private static void ReleaseBuffer(char[] buffer)
        {
            Interlocked.CompareExchange(ref _pooledBuffer, buffer, null);
        }

        internal enum LineEndingState
        {
            Unknown,
            Crlf,
            Cr,
            Lf,
            Nel,
            Ls,
            Ps,
            Inconsistent,
        }
    }
}