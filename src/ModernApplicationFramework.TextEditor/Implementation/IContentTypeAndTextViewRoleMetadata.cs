using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IContentTypeAndTextViewRoleMetadata
    {
        IEnumerable<string> TextViewRoles { get; }

        IEnumerable<string> ContentTypes { get; }
    }
}