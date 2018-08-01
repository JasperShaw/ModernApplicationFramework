using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    public interface IEncodingDetector
    {
        Encoding GetStreamEncoding(Stream stream);
    }

    [Export(typeof(IEncodingDetector))]
    [Order]
    [ContentType("text")]
    [Name("XmlEncodingDetector")]
    internal class XmlEncodingDetector : IEncodingDetector
    {
        public Encoding GetStreamEncoding(Stream stream)
        {
            var bytesReadLimit = 256;
            char curChar;
            var readBytes = new List<byte>(bytesReadLimit);

            char NextChar()
            {
                var num = bytesReadLimit-- > 0 ? stream.ReadByte() : -1;
                readBytes.Add((byte) num);
                curChar = num < 0 ? char.MinValue : (char) num;
                return curChar;
            }

            bool Func(string s)
            {
                foreach (int num in s)
                {
                    if (NextChar() != num) return false;
                }

                return true;
            }

            bool IsWhiteSpaceChar(char ch) => " \t\r\n".IndexOf(ch) >= 0;

            void Action()
            {
                do
                {
                } while (IsWhiteSpaceChar(NextChar()));
            }

            if (!Func("<?xml") || !IsWhiteSpaceChar(NextChar()))
                return null;
            while (!Func("encoding"))
            {
                while (!IsWhiteSpaceChar(curChar))
                {
                    if (curChar == char.MinValue || curChar == '?')
                        return null;
                }
            }
            Action();
            if (curChar != '=')
                return null;
            Action();
            var ch1 = curChar;
            switch (ch1)
            {
                case '"':
                case '\'':
                    var stringBuilder = new StringBuilder();
                    while (true)
                    {
                        var ch2 = NextChar();
                        switch (ch2)
                        {
                            case char.MinValue:
                            case '?':
                                goto label_11;
                            default:
                                if (ch2 != ch1)
                                {
                                    stringBuilder.Append(ch2);
                                    continue;
                                }
                                goto label_11;
                        }
                    }
                    label_11:
                    return MakeEncoding(stringBuilder.ToString(), readBytes.ToArray());
                default:
                    return null;
            }
        }

        private static Encoding MakeEncoding(string encodingName, byte[] readBytes)
        {
            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(encodingName);
            }
            catch (ArgumentException)
            {
                return null;
            }
            if (encoding == Encoding.UTF8)
                return new UTF8Encoding(false);
            if (!IsAsciiCompatible(encoding, readBytes))
                return null;
            return encoding;
        }

        private static bool IsAsciiCompatible(Encoding encoding, byte[] readBytes)
        {
            return Encoding.ASCII.GetChars(readBytes).SequenceEqual(encoding.GetChars(readBytes));
        }
    }
}