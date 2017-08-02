namespace ModernApplicationFramework.Input.Command
{
    public class CommandGestureCategory
    {
        public string Name { get; }
        
        public CommandGestureCategory(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CommandGestureCategory commandGesture))
                return false;
            return Name.Equals(commandGesture.Name);
        }

        protected bool Equals(CommandGestureCategory other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}