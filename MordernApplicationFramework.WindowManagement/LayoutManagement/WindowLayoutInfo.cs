using System;
using System.Text;
using System.Xml.Serialization;
using ModernApplicationFramework.Docking.Layout;

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

        public WindowLayoutInfo()
        {
            
        }

        internal WindowLayoutInfo(string name, int position, string key, string payload)
        {
            Name = name;
            Position = position;
            Key = key;
            Payload = payload;
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