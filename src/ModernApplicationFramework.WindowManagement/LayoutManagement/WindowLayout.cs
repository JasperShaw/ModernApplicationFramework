using System;
using System.Text;
using System.Xml.Serialization;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.WindowManagement.LayoutManagement
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Layout")]
    public class WindowLayout
    {
        private string _name;
        private int _position;
        private string _keyField;
        private string _layoutRoot;

        [XmlAttribute]
        public string Key
        {
            get => _keyField;
            set => _keyField = value;
        }

        [XmlAttribute]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [XmlAttribute]
        public int Position
        {
            get => _position;
            set => _position = value;
        }

        [XmlAttribute]
        public string Payload
        {
            get => _layoutRoot;
            set => _layoutRoot = value;
        }

        [XmlIgnore]
        internal string DecompressedPayload
        {
            get
            {
                try
                {
                    return Encoding.UTF8.GetString(GZip.Decompress(Convert.FromBase64String(Payload)));
                }
                catch
                {
                    //Ignored
                }
                return string.Empty;
            }
        }

        public WindowLayout()
        {

        }

        internal WindowLayout(string name, int position, string key, string payload, bool compress)
        {
            Name = name;
            Position = position;
            Key = key;
            Payload = compress ? CompressAndEncode(payload) : payload;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Position.ToString());
            sb.Append('|');
            sb.Append(Name);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is WindowLayout windowLayoutInfo && Position == windowLayoutInfo.Position &&
                   Name.Equals(windowLayoutInfo.Name);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private static string CompressAndEncode(string data)
        {
            return Convert.ToBase64String(GZip.Compress(Encoding.UTF8.GetBytes(data)));
        }
    }
}