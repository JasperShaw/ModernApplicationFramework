using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface IDeferrableContentTypeAndTextViewRoleMetadata : IContentTypeAndTextViewRoleMetadata
    {
        [DefaultValue(null)]
        string OptionName { get; }
    }
}