using System;

namespace ModernApplicationFramework.Utilities.Attributes
{
    public sealed class ContentTypeAttribute : MultipleBaseMetadataAttribute
    {
        public ContentTypeAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            ContentTypes = name;
        }

        public string ContentTypes { get; }
    }
}