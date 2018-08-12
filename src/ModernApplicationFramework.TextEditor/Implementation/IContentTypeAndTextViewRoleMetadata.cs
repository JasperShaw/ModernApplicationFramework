using System.Collections.Generic;

namespace ModernApplicationFramework.Editor.Implementation
{
    public interface IContentTypeAndTextViewRoleMetadata
    {
        IEnumerable<string> TextViewRoles { get; }

        IEnumerable<string> ContentTypes { get; }
    }
}