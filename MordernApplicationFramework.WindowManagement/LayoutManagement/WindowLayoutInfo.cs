using System;
using System.Text;
using System.Xml.Serialization;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Layout")]
    public class WindowLayoutInfo
    {
        private string _name;
        private int _position;
        private string _keyField;

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

        public WindowLayoutInfo()
        {
            
        }

        internal WindowLayoutInfo(string name, int position, string key)
        {
            Name = name;
            Position = position;
            Key = key;
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
            return obj is WindowLayoutInfo windowLayoutInfo && Position == windowLayoutInfo.Position && Name.Equals(windowLayoutInfo.Name);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}