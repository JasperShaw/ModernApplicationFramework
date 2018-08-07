using System.IO;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    public struct FontInfo
    {
        public string Typeface { get; set; }
        public short PointSize { get; set; }
        public byte CharSet { get; set; }

        public bool IsValid => Typeface != null;

        internal FontInfo(BinaryReader reader)
        {
            Validate.IsNotNull(reader, nameof(reader));
            if (reader.ReadBoolean())
            {
                Typeface = reader.ReadUtf8String();
                PointSize = reader.ReadInt16();
                CharSet = reader.ReadByte();
            }
            else
            {
                Typeface = null;
                PointSize = 0;
                CharSet = 0;
            }
        }

        internal void WriteToStream(BinaryWriter writer)
        {
            Validate.IsNotNull(writer, nameof(writer));
            writer.Write(IsValid);
            if (!IsValid)
                return;
            writer.WriteUtf8String(Typeface);
            writer.Write(PointSize);
            writer.Write(CharSet);
        }
    }
}