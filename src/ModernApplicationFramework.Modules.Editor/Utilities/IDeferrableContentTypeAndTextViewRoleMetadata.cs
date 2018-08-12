using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public interface IDeferrableContentTypeAndTextViewRoleMetadata : IContentTypeAndTextViewRoleMetadata
    {
        [DefaultValue(null)]
        string OptionName { get; }
    }
}