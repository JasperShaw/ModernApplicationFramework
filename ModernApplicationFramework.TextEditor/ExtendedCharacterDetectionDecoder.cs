using System;
using System.Text;

namespace ModernApplicationFramework.TextEditor
{
    internal class ExtendedCharacterDetectionDecoder : Decoder
    {
        private readonly Decoder _decoder;
        private Action _response;

        internal ExtendedCharacterDetectionDecoder(Decoder decoder, Action response)
        {
            _decoder = decoder;
            _response = response;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return _decoder.GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int chars1 = _decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
            if (_response != null)
            {
                int num = charIndex + chars1;
                for (var index = charIndex; index < num; ++index)
                {
                    if (chars[index] <= '\x007F')
                        continue;
                    _response();
                    _response = null;
                    break;
                }
            }
            return chars1;
        }
    }
}