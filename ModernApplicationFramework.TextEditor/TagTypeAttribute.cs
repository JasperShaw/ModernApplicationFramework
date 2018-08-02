using System;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    public sealed class TagTypeAttribute : MultipleBaseMetadataAttribute
    {
        public TagTypeAttribute(Type tagType)
        {
            if (tagType == null)
                throw new ArgumentNullException(nameof(tagType));
            if (!typeof(ITag).IsAssignableFrom(tagType))
                throw new ArgumentException("Given type must derive from ITag", nameof(tagType));
            TagTypes = tagType;
        }

        public Type TagTypes { get; }
    }
}