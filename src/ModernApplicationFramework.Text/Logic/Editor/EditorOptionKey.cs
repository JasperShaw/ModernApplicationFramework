namespace ModernApplicationFramework.Text.Logic.Editor
{
    public struct EditorOptionKey<T>
    {
        public EditorOptionKey(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            if (obj is EditorOptionKey<T> key)
                return key.Name == Name;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool operator ==(EditorOptionKey<T> left, EditorOptionKey<T> right)
        {
            return left.Name == right.Name;
        }

        public static bool operator !=(EditorOptionKey<T> left, EditorOptionKey<T> right)
        {
            return !(left == right);
        }
    }
}