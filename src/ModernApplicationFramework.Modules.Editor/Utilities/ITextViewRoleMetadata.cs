using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public interface ITextViewRoleMetadata
    {
        IEnumerable<string> TextViewRoles { get; }
    }
}