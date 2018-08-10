using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Classification
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