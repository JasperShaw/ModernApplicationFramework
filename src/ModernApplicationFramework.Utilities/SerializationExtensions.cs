using System.IO;
using System.Text;

namespace ModernApplicationFramework.Utilities
{
    public static class SerializationExtensions
    {
        public static string ReadUtf8String(this BinaryReader reader)
        {
            Validate.IsNotNull(reader, nameof(reader));
            int count = reader.ReadInt32();
            return Encoding.UTF8.GetString(reader.ReadBytes(count));
        }

        public static void WriteUtf8String(this BinaryWriter writer, string value)
        {
            Validate.IsNotNull(writer, nameof(writer));
            Validate.IsNotNull(value, nameof(value));
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }
    }
}
