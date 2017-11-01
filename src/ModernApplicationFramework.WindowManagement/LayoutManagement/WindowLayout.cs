using System;
using System.Text;
using System.Xml.Serialization;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.WindowManagement.LayoutManagement
{
    /// <summary>
    /// A <see cref="WindowLayout"/> contains information to store and restore window layout configurations
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Layout")]
    public class WindowLayout
    {
        private string _name;
        private int _position;
        private string _keyField;
        private string _layoutRoot;

        /// <summary>
        /// The unique key of the layout. Should not be set manually.
        /// </summary>
        [XmlAttribute]
        public string Key
        {
            get => _keyField;
            set => _keyField = value;
        }

        /// <summary>
        /// The name of the layout. Should be unique in the application
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// The index position of the layout. Used to create a mapping with the Apply#Command
        /// </summary>
        [XmlAttribute]
        public int Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// A payload string that contains the layout information
        /// </summary>
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