using System.IO;
using System.IO.Compression;

namespace ModernApplicationFramework.TextEditor
{
    internal static class Compressor
    {
        public static byte[] Compress(char[] buffer, int length)
        {
            using (var charStream = new CharStream(buffer, length))
            {
                using (var memoryStream = new MemoryStream(length / 9))
                {
                    using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                        charStream.CopyTo(deflateStream);
                    return memoryStream.GetBuffer();
                }
            }
        }

        public static void Decompress(byte[] compressed, int length, char[] decompressed)
        {
            using (var memoryStream = new MemoryStream(compressed))
            {
                using (var charStream = new CharStream(decompressed, length))
                {
                    using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                        deflateStream.CopyTo(charStream);
                }
            }
        }
    }
}