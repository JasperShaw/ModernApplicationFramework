using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public sealed class UserVisibleAttribute : SingletonBaseMetadataAttribute
    {
        public bool UserVisible { get; }

        public UserVisibleAttribute(bool userVisible)
        {
            UserVisible = userVisible;
        }
    }
}