namespace ModernApplicationFramework.Input.Command
{
    public class GestureScope
    {
        public string Name { get; }
        
        public GestureScope(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GestureScope commandGesture))
                return false;
            return Name.Equals(commandGesture.Name);
        }

        protected bool Equals(GestureScope other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}