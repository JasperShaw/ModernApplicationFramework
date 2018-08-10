using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewRoleMetadata
    {
        IEnumerable<string> TextViewRoles { get; }
    }
}