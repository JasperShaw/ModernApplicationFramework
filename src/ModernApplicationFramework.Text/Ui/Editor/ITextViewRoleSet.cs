using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewRoleSet
    {
        bool Contains(string textViewRole);

        bool ContainsAll(IEnumerable<string> textViewRoles);

        bool ContainsAny(IEnumerable<string> textViewRoles);

        ITextViewRoleSet UnionWith(ITextViewRoleSet roleSet);
    }
}