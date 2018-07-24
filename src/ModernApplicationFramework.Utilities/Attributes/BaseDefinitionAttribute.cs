using System;

namespace ModernApplicationFramework.Utilities.Attributes
{
    public sealed class BaseDefinitionAttribute : MultipleBaseMetadataAttribute
    {
        public string BaseDefinition { get; }

        public BaseDefinitionAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            BaseDefinition = name;
        }
    }
}
