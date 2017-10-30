using System;
using System.IO;
using System.Text;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.WindowManagement.LayoutManagement;

namespace ModernApplicationFramework.WindowManagement.LayoutState
{
    public static class LayoutPayloadUtilities
    {
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
