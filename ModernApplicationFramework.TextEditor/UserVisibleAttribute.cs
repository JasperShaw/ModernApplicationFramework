using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    public sealed class UserVisibleAttribute : SingletonBaseMetadataAttribute
    {
        public UserVisibleAttribute(bool userVisible)
        {
            UserVisible = userVisible;
        }

        public bool UserVisible { get; }
    }
}