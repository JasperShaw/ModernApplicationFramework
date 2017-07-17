using System.Text;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    public class WindowLayoutInfo
    {
        private string _name;
        private int _position;

        public string Name
        {
            get => _name;
            set
            {
                Validate.IsNotNullAndNotEmpty(value, "Name");
                _name = value;     
            }
        }

        public int Position
        {
            get => _position;
            set
            {
                Validate.IsWithinRange(value, 0, int.MaxValue, "Position");
                _position = value;
            }
        }

        internal WindowLayoutInfo(string name, int position)
        {
            Name = name;
            Position = position;
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
            var windowLayoutInfo = obj as WindowLayoutInfo;
            return windowLayoutInfo != null && Position == windowLayoutInfo.Position && Name.Equals(windowLayoutInfo.Name);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}