using System;

namespace ModernApplicationFramework.Utilities.Attributes
{
    public sealed class NameAttribute : SingletonBaseMetadataAttribute
    {
        public string Name { get; }

        public NameAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Length == 0)
                throw new ArgumentException();
            Name = name;
        }
    }
}
