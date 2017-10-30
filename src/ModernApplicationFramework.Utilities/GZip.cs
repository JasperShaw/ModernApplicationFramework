using System.IO;
using System.IO.Compression;

namespace ModernApplicationFramework.Utilities
{
    public static class GZip
    {
        public static byte[] Compress(byte[] data)
        {
            Validate.IsNotNull(data, nameof(data));
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    gzipStream.Write(data, 0, data.Length);
                return memoryStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data, int bufferSize = 4096)
        {
            Validate.IsNotNull(data, nameof(data));
            Validate.IsWithinRange(bufferSize, 1, int.MaxValue, nameof(bufferSize));
            using (var memoryStream1 = new MemoryStream(data))
            {
                using (var gzipStream = new GZipStream(memoryStream1, CompressionMode.Decompress))
                {
                    using (var memoryStream2 = new MemoryStream())
                    {
                        byte[] buffer = new byte[bufferSize];
                        int count;
                        do
                        {
                            count = gzipStream.Read(buffer, 0, bufferSize);
                            if (count > 0)
                                memoryStream2.Write(buffer, 0, count);
                        }
                        while (count > 0);
                        return memoryStream2.ToArray();
                    }
                }
            }
        }
    }
}
