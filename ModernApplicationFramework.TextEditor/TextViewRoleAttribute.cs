using System;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    public sealed class TextViewRoleAttribute : MultipleBaseMetadataAttribute
    {
        public TextViewRoleAttribute(string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentNullException(nameof(role));
            TextViewRoles = role;
        }

        public string TextViewRoles { get; }
    }
}