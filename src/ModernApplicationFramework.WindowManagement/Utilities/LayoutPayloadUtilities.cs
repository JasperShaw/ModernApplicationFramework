using System;
using System.IO;
using System.Text;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.WindowManagement.LayoutManagement;

namespace ModernApplicationFramework.WindowManagement.Utilities
{
    /// <summary>
    /// Helper class to work with payload data
    /// </summary>
    public static class LayoutPayloadUtilities
    {

        /// <summary>
        /// Reads a <see cref="WindowProfile"/> file and converts it to a valid payload string
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The payload</returns>
        public static string FileToPayloadData(string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fs.CopyTo(memoryStream);
                }
                memoryStream.Seek(0L, SeekOrigin.Begin);
                var byteArray = memoryStream.ToArray();
                return LayoutManagementUtilities.ConvertLayoutStreamToString(byteArray);
            }
        }

        /// <summary>
        /// Converts a <see cref="Stream"/> to a valid payload string
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <returns>The payload</returns>
        public static string StreamToPlayloadData(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                memoryStream.Seek(0L, SeekOrigin.Begin);
                var byteArray = memoryStream.ToArray();
                return LayoutManagementUtilities.ConvertLayoutStreamToString(byteArray);
            }
        }

        /// <summary>
        /// Saves a payload string to a file. An existing file will be overwritten.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="decompress">if set to <see langword="true"/> the payload data will be decompressed first. Default value is <see langword="true"/></param>
        public static void PayloadDataToFile(string filePath, string payload, bool decompress = true)
        {
            var data = payload;
            if (decompress)
                data = Encoding.UTF8.GetString(GZip.Decompress(Convert.FromBase64String(payload)));
            using (var stream = LayoutManagementUtilities.ConvertLayoutPayloadToStream(data))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
    }
}
