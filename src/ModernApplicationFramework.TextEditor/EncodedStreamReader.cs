using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class EncodedStreamReader
    {
        public static Encoding DetectEncoding(Stream stream, List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>> encodingDetectorExtensions, GuardedOperations guardedOperations)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            var position = stream.Position;
            var encoding = CheckForBoM(stream, out var isStreamEmpty);
            if (encoding == null && !isStreamEmpty)
            {
                encoding = SniffForEncoding(stream, encodingDetectorExtensions, guardedOperations);
                stream.Position = position;
            }
            return encoding;
        }

        internal static Encoding CheckForBoM(Stream stream, out bool isStreamEmpty)
        {
            var position = stream.Position;
            using (StreamReader streamReader = new NonStreamClosingStreamReader(stream, Encoding.ASCII, true))
            {
                var num = streamReader.Read();
                isStreamEmpty = num == -1;
                stream.Position = position;
                if (streamReader.CurrentEncoding == Encoding.ASCII)
                    return null;
                return streamReader.CurrentEncoding;
            }
        }

        private static Encoding SniffForEncoding(Stream stream, List<Lazy<IEncodingDetector, IEncodingDetectorMetadata>> orderedEncodingDetectors, GuardedOperations guardedOperations)
        {
            var position = stream.Position;
            foreach (var encodingDetector in orderedEncodingDetectors)
            {
                Encoding encoding = null;
                try
                {
                    encoding = encodingDetector.Value.GetStreamEncoding(stream);
                }
                catch (Exception ex)
                {
                    guardedOperations.HandleException(encodingDetector, ex);
                }
                stream.Position = position;
                if (encoding != null)
                    return encoding;
            }
            return null;
        }

        internal class NonStreamClosingStreamReader : StreamReader
        {
            internal NonStreamClosingStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
                : base(stream, encoding, detectEncodingFromByteOrderMarks)
            {
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(false);
            }
        }
    }
}